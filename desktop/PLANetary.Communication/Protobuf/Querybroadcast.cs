// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: querybroadcast.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace PLANetary.Communication.Protobuf {

  /// <summary>Holder for reflection information generated from querybroadcast.proto</summary>
  public static partial class QuerybroadcastReflection {

    #region Descriptor
    /// <summary>File descriptor for querybroadcast.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static QuerybroadcastReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChRxdWVyeWJyb2FkY2FzdC5wcm90bxoLcXVlcnkucHJvdG8aEGlkZW50aWZp",
            "ZXIucHJvdG8idwoOUXVlcnlCcm9hZGNhc3QSGQoIcGFyZW50SWQYASABKAsy",
            "By5Ob2RlSWQSFgoOZGlzdGFuY2VUb1NpbmsYAiABKA0SGwoTcGFyZW50SXNQ",
            "YXJ0T2ZRdWVyeRgDIAEoCBIVCgVxdWVyeRgEIAEoCzIGLlF1ZXJ5QiOqAiBQ",
            "TEFOZXRhcnkuQ29tbXVuaWNhdGlvbi5Qcm90b2J1ZmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::PLANetary.Communication.Protobuf.QueryReflection.Descriptor, global::PLANetary.Communication.Protobuf.IdentifierReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::PLANetary.Communication.Protobuf.QueryBroadcast), global::PLANetary.Communication.Protobuf.QueryBroadcast.Parser, new[]{ "ParentId", "DistanceToSink", "ParentIsPartOfQuery", "Query" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class QueryBroadcast : pb::IMessage<QueryBroadcast> {
    private static readonly pb::MessageParser<QueryBroadcast> _parser = new pb::MessageParser<QueryBroadcast>(() => new QueryBroadcast());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<QueryBroadcast> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::PLANetary.Communication.Protobuf.QuerybroadcastReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public QueryBroadcast() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public QueryBroadcast(QueryBroadcast other) : this() {
      parentId_ = other.parentId_ != null ? other.parentId_.Clone() : null;
      distanceToSink_ = other.distanceToSink_;
      parentIsPartOfQuery_ = other.parentIsPartOfQuery_;
      query_ = other.query_ != null ? other.query_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public QueryBroadcast Clone() {
      return new QueryBroadcast(this);
    }

    /// <summary>Field number for the "parentId" field.</summary>
    public const int ParentIdFieldNumber = 1;
    private global::PLANetary.Communication.Protobuf.NodeId parentId_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::PLANetary.Communication.Protobuf.NodeId ParentId {
      get { return parentId_; }
      set {
        parentId_ = value;
      }
    }

    /// <summary>Field number for the "distanceToSink" field.</summary>
    public const int DistanceToSinkFieldNumber = 2;
    private uint distanceToSink_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint DistanceToSink {
      get { return distanceToSink_; }
      set {
        distanceToSink_ = value;
      }
    }

    /// <summary>Field number for the "parentIsPartOfQuery" field.</summary>
    public const int ParentIsPartOfQueryFieldNumber = 3;
    private bool parentIsPartOfQuery_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool ParentIsPartOfQuery {
      get { return parentIsPartOfQuery_; }
      set {
        parentIsPartOfQuery_ = value;
      }
    }

    /// <summary>Field number for the "query" field.</summary>
    public const int QueryFieldNumber = 4;
    private global::PLANetary.Communication.Protobuf.Query query_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::PLANetary.Communication.Protobuf.Query Query {
      get { return query_; }
      set {
        query_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as QueryBroadcast);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(QueryBroadcast other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ParentId, other.ParentId)) return false;
      if (DistanceToSink != other.DistanceToSink) return false;
      if (ParentIsPartOfQuery != other.ParentIsPartOfQuery) return false;
      if (!object.Equals(Query, other.Query)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (parentId_ != null) hash ^= ParentId.GetHashCode();
      if (DistanceToSink != 0) hash ^= DistanceToSink.GetHashCode();
      if (ParentIsPartOfQuery != false) hash ^= ParentIsPartOfQuery.GetHashCode();
      if (query_ != null) hash ^= Query.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (parentId_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ParentId);
      }
      if (DistanceToSink != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(DistanceToSink);
      }
      if (ParentIsPartOfQuery != false) {
        output.WriteRawTag(24);
        output.WriteBool(ParentIsPartOfQuery);
      }
      if (query_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Query);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (parentId_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ParentId);
      }
      if (DistanceToSink != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(DistanceToSink);
      }
      if (ParentIsPartOfQuery != false) {
        size += 1 + 1;
      }
      if (query_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Query);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(QueryBroadcast other) {
      if (other == null) {
        return;
      }
      if (other.parentId_ != null) {
        if (parentId_ == null) {
          parentId_ = new global::PLANetary.Communication.Protobuf.NodeId();
        }
        ParentId.MergeFrom(other.ParentId);
      }
      if (other.DistanceToSink != 0) {
        DistanceToSink = other.DistanceToSink;
      }
      if (other.ParentIsPartOfQuery != false) {
        ParentIsPartOfQuery = other.ParentIsPartOfQuery;
      }
      if (other.query_ != null) {
        if (query_ == null) {
          query_ = new global::PLANetary.Communication.Protobuf.Query();
        }
        Query.MergeFrom(other.Query);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (parentId_ == null) {
              parentId_ = new global::PLANetary.Communication.Protobuf.NodeId();
            }
            input.ReadMessage(parentId_);
            break;
          }
          case 16: {
            DistanceToSink = input.ReadUInt32();
            break;
          }
          case 24: {
            ParentIsPartOfQuery = input.ReadBool();
            break;
          }
          case 34: {
            if (query_ == null) {
              query_ = new global::PLANetary.Communication.Protobuf.Query();
            }
            input.ReadMessage(query_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code