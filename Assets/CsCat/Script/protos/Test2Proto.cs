// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Test2Proto.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from Test2Proto.proto</summary>
public static partial class Test2ProtoReflection {

  #region Descriptor
  /// <summary>File descriptor for Test2Proto.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static Test2ProtoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChBUZXN0MlByb3RvLnByb3RvIh0KClRlc3QyUHJvdG8SDwoHYWNjb3VudBgB",
          "IAEoCWIGcHJvdG8z"));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::Test2Proto), global::Test2Proto.Parser, new[]{ "Account" }, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class Test2Proto : pb::IMessage<Test2Proto> {
  private static readonly pb::MessageParser<Test2Proto> _parser = new pb::MessageParser<Test2Proto>(() => new Test2Proto());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<Test2Proto> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::Test2ProtoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Test2Proto() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Test2Proto(Test2Proto other) : this() {
    account_ = other.account_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public Test2Proto Clone() {
    return new Test2Proto(this);
  }

  /// <summary>Field number for the "account" field.</summary>
  public const int AccountFieldNumber = 1;
  private string account_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Account {
    get { return account_; }
    set {
      account_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as Test2Proto);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(Test2Proto other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Account != other.Account) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Account.Length != 0) hash ^= Account.GetHashCode();
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
    if (Account.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Account);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Account.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Account);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(Test2Proto other) {
    if (other == null) {
      return;
    }
    if (other.Account.Length != 0) {
      Account = other.Account;
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
          Account = input.ReadString();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
