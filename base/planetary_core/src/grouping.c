
#include <planetary/planetary_config.h>
#include <planetary/typedefs.h>
#include <planetary/querytypes.h>
#include <planetary/utils.h>

// ***************************************
// ******* combining & grouping **********
// ***************************************

#if ALLOW_GROUPING == 1

// returns the index of resultset in the groups array according to query's selections or -1 if it is not present
int getGroupIndex(QuerySlot* query, ResultRow* resultrow, GroupBy* groups, int group_count)
{
	int idx = -1;
	int i;
	int g;
	bool areTheSame;
	bool hasGroups;

	for (i = 0; i < group_count; i++)
	{
		areTheSame = true;
		hasGroups = false;

		for (g = 0; g < query->queryData.actions_count; g++)
		{
			if (query->queryData.actions[g].which_content == ActionType_SELECTOR && query->queryData.actions[g].content.selector.type == SelectorType_GROUP_BY)
			{
				hasGroups = true;

				if (!compare_float(resultrow->values[g], groups[i].values[g]))
				{
					areTheSame = false;
					break;
				}
			}
		}

		if (!hasGroups)
			break; // no group statements -> no grouping

		if (areTheSame)
			return i;
	}

	return idx;
}

void addGroup(QuerySlot* query, ResultRow* resultrow, GroupBy* group_buffer, int addIndex)
{
	int g;
	for (g = 0; g < query->queryData.actions_count; g++)
		if (query->queryData.actions[g].which_content == ActionType_SELECTOR && query->queryData.actions[g].content.selector.type == SelectorType_GROUP_BY)
		{
			group_buffer[addIndex].values[g] = resultrow->values[g];
		}
}

// returns the group count (unique SEL_GROUP lines) of query results and writes them into group_buffer
int findResultGroups(QuerySlot* query, GroupBy* group_buffer)
{
	int groupCount = 0;
	int r;
	int g;

	if (query->resultset.rows_count == 0)
		return 0;

	for (r = 0; r < query->resultset.rows_count; r++)
	{
		g = getGroupIndex(query, &query->resultset.rows[r], group_buffer, groupCount);

		if (g == -1)
		{
			addGroup(query, &query->resultset.rows[r], group_buffer, groupCount);
			// add the new group
			groupCount++;
		}
	}

	return groupCount;
}

// returns true if the query contains grouping statements
bool needsGrouping(QuerySlot* query)
{
	int s;
	for (s = 0; s < query->queryData.actions_count; s++)
		if (query->queryData.actions[s].which_content == ActionType_SELECTOR &&
			query->queryData.actions[s].content.selector.type == SelectorType_GROUP_BY)
			return true;

	return false;
}

void combineResults(QuerySlot* query)
{
	// combine the results
	float tmp_val;
	int combinedNewResults = 0;
	int r;
	int v;
	int i;
	int a;
	int g;

	ResultRow results[MAX_QUERY_RESULTS];

	if (query->resultset.rows_count <= 1)
		// empty or single result set => nothing to combine
		return;

	// if one value is SEL_SINGLE all others need to be SEL_SINGLE, too -> no combination possible
	if (query->queryData.actions[0].content.selector.type != SelectorType_SINGLE)
	{
		combinedNewResults = 1;
		GroupBy group_buffer[MAX_QUERY_RESULTS];

		// grouping, find groups
		if (needsGrouping(query))
		{
			combinedNewResults = findResultGroups(query, group_buffer);
		}

		if (combinedNewResults == 1)
		{
			results[0].numberOfNodes = 0;

			for (r = 0; r < query->resultset.rows_count; r++)
			{
				results[0].numberOfNodes += query->resultset.rows[r].numberOfNodes;
			}

			// copy group values
			for (v = 0; v < query->queryData.actions_count; v++)
			{
				if (query->queryData.actions[v].which_content == ActionType_SELECTOR && query->queryData.actions[v].content.selector.type == SelectorType_GROUP_BY)
					results[0].values[v] = group_buffer[0].values[v];
			}
		}
		else
		{
			// fill group non aggregate values (SEL_GROUP_BY)
			for (i = 0; i < combinedNewResults; i++)
			{
				results[i].numberOfNodes = 0;
				for (v = 0; v < query->queryData.actions_count; v++)
				{
					if (query->queryData.actions[v].which_content == ActionType_SELECTOR && query->queryData.actions[v].content.selector.type == SelectorType_GROUP_BY)
						results[i].values[v] = group_buffer[i].values[v];
				}
			}

			// sum up node count for each group
			for (r = 0; r < query->resultset.rows_count; r++)
			{
				g = getGroupIndex(query, &query->resultset.rows[r], group_buffer, combinedNewResults);
				if (g != -1)
					results[g].numberOfNodes += query->resultset.rows[r].numberOfNodes;
			}
		}

		// aggregate all values
		for (g = 0; g < combinedNewResults; g++)
		{
			for (a = 0; a < query->queryData.actions_count; a++)
			{
				if (query->queryData.actions[a].which_content != ActionType_SELECTOR || query->queryData.actions[a].content.selector.type == SelectorType_GROUP_BY)
					continue;

				switch (query->queryData.actions[a].content.selector.type)
				{
				case SelectorType_SUM:
				case SelectorType_MAX:
				case SelectorType_COUNT:
					tmp_val = 0;
					break;
				case SelectorType_MIN:
					tmp_val = 65535;
					break;
				case SelectorType_AVG:
					tmp_val = 0;
					break;
				}

				// combine the values of the different result sets
				for (r = 0; r < query->resultset.rows_count; r++)
				{
					// does the resultset belong to the current group?
					if (combinedNewResults == 1 || getGroupIndex(query, &query->resultset.rows[r], group_buffer, combinedNewResults) == g)
					{
						if (query->queryData.actions[a].which_content == ActionType_SELECTOR)
						{
							switch (query->queryData.actions[a].content.selector.type)
							{
							case SelectorType_SUM:
								tmp_val += query->resultset.rows[r].values[a];
								break;

							case SelectorType_COUNT:
								tmp_val += query->resultset.rows[r].numberOfNodes;
								break;

							case SelectorType_MAX:
								if (query->resultset.rows[r].values[a] > tmp_val)
									tmp_val = query->resultset.rows[r].values[a];
								break;

							case SelectorType_MIN:
								if (query->resultset.rows[r].values[a] < tmp_val)
									tmp_val = query->resultset.rows[r].values[a];
								break;

							case SelectorType_AVG:
								// sum up weighted average
								tmp_val += query->resultset.rows[r].numberOfNodes * query->resultset.rows[r].values[a];
								break;
							}
						}
					}

					// combine the result
					switch (query->queryData.actions[a].content.selector.type)
					{
					case SelectorType_AVG:
						// calculate average
						results[g].values[a] = tmp_val / results[g].numberOfNodes;
						break;
					default:
						results[g].values[a] = tmp_val;
						break;
					}
				}
			}
		}
	}

	// did we combine anything?
	if (combinedNewResults > 0)
	{
		// replace the old results with the new
		query->resultset.rows_count = combinedNewResults;
		for (r = 0; r < combinedNewResults; r++)
		{
			query->resultset.rows[r].numberOfNodes = results[r].numberOfNodes;
			for (v = 0; v < query->queryData.actions_count; v++)
			{
				if (query->queryData.actions[v].which_content == ActionType_SELECTOR)
					query->resultset.rows[r].values[v] = results[r].values[v];
			}
		}
	}
}

#endif