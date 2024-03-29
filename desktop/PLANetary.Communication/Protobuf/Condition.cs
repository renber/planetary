// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: condition.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace PLANetary.Communication.Protobuf {

  /// <summary>Holder for reflection information generated from condition.proto</summary>
  public static partial class ConditionReflection {

    #region Descriptor
    /// <summary>File descriptor for condition.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ConditionReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9jb25kaXRpb24ucHJvdG8aEGlkZW50aWZpZXIucHJvdG8iVwoOQ29uZGl0",
            "aW9uR3JvdXASJQoNY29uZGl0aW9uTGluaxgBIAEoDjIOLkNvbmRpdGlvbkxp",
            "bmsSHgoKY29uZGl0aW9ucxgCIAMoCzIKLkNvbmRpdGlvbiJXCglDb25kaXRp",
            "b24SHwoKaWRlbnRpZmllchgBIAEoCzILLklkZW50aWZpZXISGgoCb3AYAiAB",
            "KA4yDi5WYWx1ZU9wZXJhdG9yEg0KBXZhbHVlGAMgASgCKiAKDUNvbmRpdGlv",
            "bkxpbmsSBwoDQU5EEAASBgoCT1IQASpjCg1WYWx1ZU9wZXJhdG9yEgkKBUVR",
            "VUFMEAASCwoHR1JFQVRFUhABEhQKEEdSRUFURVJfT1JfRVFVQUwQAhIICgRM",
            "RVNTEAMSEQoNTEVTU19PUl9FUVVBTBAEEgcKA05PVBAFQiOqAiBQTEFOZXRh",
            "cnkuQ29tbXVuaWNhdGlvbi5Qcm90b2J1ZmIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::PLANetary.Communication.Protobuf.IdentifierReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::PLANetary.Communication.Protobuf.ConditionLink), typeof(global::PLANetary.Communication.Protobuf.ValueOperator), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::PLANetary.Communication.Protobuf.ConditionGroup), global::PLANetary.Communication.Protobuf.ConditionGroup.Parser, new[]{ "ConditionLink", "Conditions" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::PLANetary.Communication.Protobuf.Condition), global::PLANetary.Communication.Protobuf.Condition.Parser, new[]{ "Identifier", "Op", "Value" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum ConditionLink {
    [pbr::OriginalName("AND")] And = 0,
    [pbr::OriginalName("OR")] Or = 1,
  }

  public enum ValueOperator {
    [pbr::OriginalName("EQUAL")] Equal = 0,
    [pbr::OriginalName("GREATER")] Greater = 1,
    [pbr::OriginalName("GREATER_OR_EQUAL")] GreaterOrEqual = 2,
    [pbr::OriginalName("LESS")] Less = 3,
    [pbr::OriginalName("LESS_OR_EQUAL")] LessOrEqual = 4,
    [pbr::OriginalName("NOT")] Not = 5,
  }

  #endregion

  #region Messages
  public sealed partial class ConditionGroup : pb::IMessage<ConditionGroup> {
    private static readonly pb::MessageParser<ConditionGroup> _parser = new pb::MessageParser<ConditionGroup>(() => new ConditionGroup());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ConditionGroup> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::PLANetary.Communication.Protobuf.ConditionReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ConditionGroup() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ConditionGroup(ConditionGroup other) : this() {
      conditionLink_ = other.conditionLink_;
      conditions_ = other.conditions_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ConditionGroup Clone() {
      return new ConditionGroup(this);
    }

    /// <summary>Field number for the "conditionLink" field.</summary>
    public const int ConditionLinkFieldNumber = 1;
    private global::PLANetary.Communication.Protobuf.ConditionLink conditionLink_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::PLANetary.Communication.Protobuf.ConditionLink ConditionLink {
      get { return conditionLink_; }
      set {
        conditionLink_ = value;
      }
    }

    /// <summary>Field number for the "conditions" field.</summary>
    public const int ConditionsFieldNumber = 2;
    private static readonly pb::FieldCodec<global::PLANetary.Communication.Protobuf.Condition> _repeated_conditions_codec
        = pb::FieldCodec.ForMessage(18, global::PLANetary.Communication.Protobuf.Condition.Parser);
    private readonly pbc::RepeatedField<global::PLANetary.Communication.Protobuf.Condition> conditions_ = new pbc::RepeatedField<global::PLANetary.Communication.Protobuf.Condition>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::PLANetary.Communication.Protobuf.Condition> Conditions {
      get { return conditions_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ConditionGroup);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ConditionGroup other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ConditionLink != other.ConditionLink) return false;
      if(!conditions_.Equals(other.conditions_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (ConditionLink != 0) hash ^= ConditionLink.GetHashCode();
      hash ^= conditions_.GetHashCode();
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
      if (ConditionLink != 0) {
        output.WriteRawTag(8);
        output.WriteEnum((int) ConditionLink);
      }
      conditions_.WriteTo(output, _repeated_conditions_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ConditionLink != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) ConditionLink);
      }
      size += conditions_.CalculateSize(_repeated_conditions_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ConditionGroup other) {
      if (other == null) {
        return;
      }
      if (other.ConditionLink != 0) {
        ConditionLink = other.ConditionLink;
      }
      conditions_.Add(other.conditions_);
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
          case 8: {
            conditionLink_ = (global::PLANetary.Communication.Protobuf.ConditionLink) input.ReadEnum();
            break;
          }
          case 18: {
            conditions_.AddEntriesFrom(input, _repeated_conditions_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class Condition : pb::IMessage<Condition> {
    private static readonly pb::MessageParser<Condition> _parser = new pb::MessageParser<Condition>(() => new Condition());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Condition> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::PLANetary.Communication.Protobuf.ConditionReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Condition() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Condition(Condition other) : this() {
      identifier_ = other.identifier_ != null ? other.identifier_.Clone() : null;
      op_ = other.op_;
      value_ = other.value_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Condition Clone() {
      return new Condition(this);
    }

    /// <summary>Field number for the "identifier" field.</summary>
    public const int IdentifierFieldNumber = 1;
    private global::PLANetary.Communication.Protobuf.Identifier identifier_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::PLANetary.Communication.Protobuf.Identifier Identifier {
      get { return identifier_; }
      set {
        identifier_ = value;
      }
    }

    /// <summary>Field number for the "op" field.</summary>
    public const int OpFieldNumber = 2;
    private global::PLANetary.Communication.Protobuf.ValueOperator op_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::PLANetary.Communication.Protobuf.ValueOperator Op {
      get { return op_; }
      set {
        op_ = value;
      }
    }

    /// <summary>Field number for the "value" field.</summary>
    public const int ValueFieldNumber = 3;
    private float value_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public float Value {
      get { return value_; }
      set {
        value_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Condition);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Condition other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Identifier, other.Identifier)) return false;
      if (Op != other.Op) return false;
      if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Value, other.Value)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (identifier_ != null) hash ^= Identifier.GetHashCode();
      if (Op != 0) hash ^= Op.GetHashCode();
      if (Value != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Value);
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
      if (identifier_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Identifier);
      }
      if (Op != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) Op);
      }
      if (Value != 0F) {
        output.WriteRawTag(29);
        output.WriteFloat(Value);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (identifier_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Identifier);
      }
      if (Op != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Op);
      }
      if (Value != 0F) {
        size += 1 + 4;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Condition other) {
      if (other == null) {
        return;
      }
      if (other.identifier_ != null) {
        if (identifier_ == null) {
          identifier_ = new global::PLANetary.Communication.Protobuf.Identifier();
        }
        Identifier.MergeFrom(other.Identifier);
      }
      if (other.Op != 0) {
        Op = other.Op;
      }
      if (other.Value != 0F) {
        Value = other.Value;
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
            if (identifier_ == null) {
              identifier_ = new global::PLANetary.Communication.Protobuf.Identifier();
            }
            input.ReadMessage(identifier_);
            break;
          }
          case 16: {
            op_ = (global::PLANetary.Communication.Protobuf.ValueOperator) input.ReadEnum();
            break;
          }
          case 29: {
            Value = input.ReadFloat();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
