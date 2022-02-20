//AutoGen. DO NOT EDIT!!!
//ExportFrom DW单位表.xlsx[单位表]
using System;
using System.Collections.Generic;
using LitJson;
namespace CsCat{
  public class CfgUnit {
    protected CfgUnit () {}
    public static CfgUnit Instance => instance;
    protected static CfgUnit instance = new CfgUnit();
    protected CfgUnitRoot root;
    public void Parse(string jsonStr) { this.root=JsonMapper.ToObject<CfgUnitRoot>(jsonStr);}
    public List<CfgUnitData> All(){ return this.root.data_list; }
    public CfgUnitData Get(int index){ return this.root.data_list[index]; }
    public CfgUnitData GetById(string id){
      string key = id.ToString();
      return this.Get(this.root.index_dict.unique.id[key]);
    }
    public bool IsContainsKeyById(string id){
      string key = id.ToString();
      return this.root.index_dict.unique.id.ContainsKey(key);
    }
  }
  public class CfgUnitRoot{
    public List<CfgUnitData> data_list { get; set; }
    public CfgUnitIndexData index_dict { get; set; }
  }
  public partial class CfgUnitData {
    /*ID*/
    public string id { get; set; }
    /*名字*/
    public string name { get; set; }
    /*类型*/
    public string type { get; set; }
    /*y轴偏移*/
    public float offsetYy { get; set; }
    /*半径*/
    public float radius { get; set; }
    /*缩放*/
    public float scale { get; set; }
    /*一轮走路动画走多远*/
    public float walkStepLength { get; set; }
    /*模型路径*/
    public string modelPath { get; set; }
    /*普攻ids*/
    public LitJson.JsonData normalAttackIds { get; set; }
    private string[] __normalAttackIds;
    public string[] _normalAttackIds {
      get{
        if(__normalAttackIds == default(string[])) __normalAttackIds = normalAttackIds.To<string[]>();
        return __normalAttackIds;
      }
    }
    /*技能ids*/
    public LitJson.JsonData skillIds { get; set; }
    private string[] __skillIds;
    public string[] _skillIds {
      get{
        if(__skillIds == default(string[])) __skillIds = skillIds.To<string[]>();
        return __skillIds;
      }
    }
    /*ai实现类(lua)*/
    public string aiClassPathLua { get; set; }
    /*ai实现类(cs)*/
    public string aiClassPathCs { get; set; }
    /*死亡后是否保留尸体*/
    public bool isKeepDeadBody { get; set; }
    /*死亡后多少秒才销毁尸体*/
    public float deadBodyDealy { get; set; }
    /*死亡时候触发的特效id*/
    public string deathEffectId { get; set; }
    /*被动buff ids*/
    public LitJson.JsonData passiveBuffIds { get; set; }
    private string[] __passiveBuffIds;
    public string[] _passiveBuffIds {
      get{
        if(__passiveBuffIds == default(string[])) __passiveBuffIds = passiveBuffIds.To<string[]>();
        return __passiveBuffIds;
      }
    }
  }
  public class CfgUnitIndexData {
    public CfgUnitIndexUniqueData unique{ get; set; }
  }
  public class CfgUnitIndexUniqueData {
    public Dictionary<string, int> id { get; set; } 
  }
}