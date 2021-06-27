// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: TestProto.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Com.Test {

  /// <summary>Holder for reflection information generated from TestProto.proto</summary>
  public static partial class TestProtoReflection {

    #region Descriptor
    /// <summary>File descriptor for TestProto.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static TestProtoReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9UZXN0UHJvdG8ucHJvdG8SCGNvbS50ZXN0GhJUZXN0U3ViUHJvdG8ucHJv",
            "dG8iyAEKCVRlc3RQcm90bxIPCgdhY2NvdW50GAEgASgJEhAKCHBhc3N3b3Jk",
            "GAIgASgJEisKBGRpY3QYAyADKAsyHS5jb20udGVzdC5UZXN0UHJvdG8uRGlj",
            "dEVudHJ5EhAKCGFkcmVzc2VzGAQgAygJEiwKDHRlc3RTdWJQcm90bxgFIAEo",
            "CzIWLmNvbS50ZXN0LlRlc3RTdWJQcm90bxorCglEaWN0RW50cnkSCwoDa2V5",
            "GAEgASgJEg0KBXZhbHVlGAIgASgJOgI4AWIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Com.Test.TestSubProtoReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Com.Test.TestProto), global::Com.Test.TestProto.Parser, new[]{ "Account", "Password", "Dict", "Adresses", "TestSubProto" }, null, null, new pbr::GeneratedClrTypeInfo[] { null, })
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class TestProto : pb::IMessage<TestProto> {
    private static readonly pb::MessageParser<TestProto> _parser = new pb::MessageParser<TestProto>(() => new TestProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<TestProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Com.Test.TestProtoReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TestProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TestProto(TestProto other) : this() {
      account_ = other.account_;
      password_ = other.password_;
      dict_ = other.dict_.Clone();
      adresses_ = other.adresses_.Clone();
      testSubProto_ = other.testSubProto_ != null ? other.testSubProto_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public TestProto Clone() {
      return new TestProto(this);
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

    /// <summary>Field number for the "password" field.</summary>
    public const int PasswordFieldNumber = 2;
    private string password_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Password {
      get { return password_; }
      set {
        password_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "dict" field.</summary>
    public const int DictFieldNumber = 3;
    private static readonly pbc::MapField<string, string>.Codec _map_dict_codec
        = new pbc::MapField<string, string>.Codec(pb::FieldCodec.ForString(10), pb::FieldCodec.ForString(18), 26);
    private readonly pbc::MapField<string, string> dict_ = new pbc::MapField<string, string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::MapField<string, string> Dict {
      get { return dict_; }
    }

    /// <summary>Field number for the "adresses" field.</summary>
    public const int AdressesFieldNumber = 4;
    private static readonly pb::FieldCodec<string> _repeated_adresses_codec
        = pb::FieldCodec.ForString(34);
    private readonly pbc::RepeatedField<string> adresses_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> Adresses {
      get { return adresses_; }
    }

    /// <summary>Field number for the "testSubProto" field.</summary>
    public const int TestSubProtoFieldNumber = 5;
    private global::Com.Test.TestSubProto testSubProto_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Com.Test.TestSubProto TestSubProto {
      get { return testSubProto_; }
      set {
        testSubProto_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as TestProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(TestProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Account != other.Account) return false;
      if (Password != other.Password) return false;
      if (!Dict.Equals(other.Dict)) return false;
      if(!adresses_.Equals(other.adresses_)) return false;
      if (!object.Equals(TestSubProto, other.TestSubProto)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Account.Length != 0) hash ^= Account.GetHashCode();
      if (Password.Length != 0) hash ^= Password.GetHashCode();
      hash ^= Dict.GetHashCode();
      hash ^= adresses_.GetHashCode();
      if (testSubProto_ != null) hash ^= TestSubProto.GetHashCode();
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
      if (Password.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Password);
      }
      dict_.WriteTo(output, _map_dict_codec);
      adresses_.WriteTo(output, _repeated_adresses_codec);
      if (testSubProto_ != null) {
        output.WriteRawTag(42);
        output.WriteMessage(TestSubProto);
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
      if (Password.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Password);
      }
      size += dict_.CalculateSize(_map_dict_codec);
      size += adresses_.CalculateSize(_repeated_adresses_codec);
      if (testSubProto_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(TestSubProto);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(TestProto other) {
      if (other == null) {
        return;
      }
      if (other.Account.Length != 0) {
        Account = other.Account;
      }
      if (other.Password.Length != 0) {
        Password = other.Password;
      }
      dict_.Add(other.dict_);
      adresses_.Add(other.adresses_);
      if (other.testSubProto_ != null) {
        if (testSubProto_ == null) {
          testSubProto_ = new global::Com.Test.TestSubProto();
        }
        TestSubProto.MergeFrom(other.TestSubProto);
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
          case 18: {
            Password = input.ReadString();
            break;
          }
          case 26: {
            dict_.AddEntriesFrom(input, _map_dict_codec);
            break;
          }
          case 34: {
            adresses_.AddEntriesFrom(input, _repeated_adresses_codec);
            break;
          }
          case 42: {
            if (testSubProto_ == null) {
              testSubProto_ = new global::Com.Test.TestSubProto();
            }
            input.ReadMessage(testSubProto_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
