############作用######################################
## 收集需要切换多语言字符串
## 将收集需要切换多语言字符串写到export_translation_file_path中
######################################################
import sys
from translation.TranslationConst import *
from pythoncat.util.ExcelUtil import *
from pythoncat.util.FileUtil import *
from pythoncat.util.StringUtil import *
from export_xlsx.ExportXlsxConst import *
from export_xlsx.ExportXlsxUtil import *

sys.path.append(os.path.dirname(os.path.realpath(__file__ + "/..")))

#收集需要转换的字符串
def CollectTranslationIds():
  translation_id_set = set()
  CollectTranslationId(TranslationConst.Cs_Translation_Root_Dir_Path, FilterCSFile, TranslationConst.Cs_Match_Pattern_List, translation_id_set)
  CollectTranslationId(TranslationConst.Lua_Translation_Root_Dir_Path, FilterLuaFile, TranslationConst.Lua_Match_Pattern_List, translation_id_set)
  CollectExcelTranslationId(TranslationConst.Excel_Translation_Root_Dir_Path,FilterExcelFile,translation_id_set)
  CollectPythonExcelTranslationIds(translation_id_set, TranslationConst.UI_String_File_Path)
  CollectPythonExcelTranslationIds(translation_id_set, TranslationConst.Custom_String_File_Path)
  return translation_id_set

def FilterCSFile(file_path):
  if not file_path.endswith(".cs"):
    return True
  for ignore_translate_file_path in TranslationConst.Ignore_Translate_File_Path_Dict:
    if file_path.find(ignore_translate_file_path) != -1:
      return True
  return False

def FilterLuaFile(file_path):
  if not file_path.endswith(".lua.txt"):
    return True
  for ignore_translate_file_path in TranslationConst.Ignore_Translate_File_Path_Dict:
    if file_path.find(ignore_translate_file_path) != -1:
      return True
  return False

def FilterExcelFile(file_path):
  if not file_path.endswith(".xlsx") or os.path.basename(file_path).startswith("~$"):
    return True
  return False

#收集文件中需要转换的字符串
def CollectTranslationId(translation_root_dir_path, FilterFile,match_pattern_list,translation_id_set):
  file_path_list = FileUtil.GetFilePathList(translation_root_dir_path, FilterFile)
  # print(file_path_list)
  for file_path in file_path_list:
    # if file_path.find("Assets\Lua\luacat\Client.lua.txt") ==-1:
    #   continue
    content = FileUtil.ReadFile(file_path)
    # print(content)
    translation_id_list = GetTranslationIdList(content,match_pattern_list)
    if(len(translation_id_list) == 0):
      continue
    for translation_id in translation_id_list:
      translation_id = StringUtil.Escape(translation_id)
      translation_id_set.add(translation_id)
  # print(translation_id_set)

#收集Excel文件中需要转换的字符串
def CollectExcelTranslationId(translation_root_dir_path, FilterFile,translation_id_set):
  file_path_list = FileUtil.GetFilePathList(translation_root_dir_path, FilterFile)
  # print(file_path_list)
  fieldInfo_type_row = ExportXlsxConst.Sheet_FieldInfo_Type_Row
  data_start_row = ExportXlsxConst.Sheet_Data_Start_Row
  translation_id_set = set()
  for file_path in file_path_list:
    wrokbook = load_workbook(file_path, read_only=True, data_only=True)
    sheet_count = len(wrokbook.sheetnames)
    for sheet_index in range(0, sheet_count):
      sheet = wrokbook.worksheets[sheet_index]
      if ExportXlsxUtil.IsExportSheet(sheet):
        fieldInfo_type_line = ExcelUtil.ReadExcelAsLine(sheet, fieldInfo_type_row)
        data_line_list = ExcelUtil.ReadExcelAsLineList(sheet, data_start_row)
        for column in range(0, len(fieldInfo_type_line)):  # 列
          type = fieldInfo_type_line[column]
          if type is None or str.lower(type) != ExportXlsxConst.Sheet_FieldInfo_Type_Translation:  # 只收集type为translation的字段
            continue
          for row in range(0, len(data_line_list)):  # 行
            field_value = data_line_list[row][column]
            if field_value is None or StringUtil.IsNoneOrEmpty(field_value) or field_value == "none":
              continue
            translation_id = field_value
            translation_id_set.add(translation_id)

#用match_pattern_list收集content中的多语言字符串
def GetTranslationIdList(content, match_pattern_list):
  match_dict = {}
  length = len(content)
  index = 0
  translation_id_list = []
  #先搜索一次
  for (key, pattern) in match_pattern_list:
    match = pattern.search(content)
    match_dict[key] = match

  while True:
    min_match_key = None
    min_match_start_index = length
    for (key, pattern) in match_pattern_list:
      match = match_dict[key]

      if not match:
        continue
      if match.start() < index:
        match = pattern.search(content,index)
        match_dict[key] = match
      if match and match.start() < min_match_start_index:
        min_match_key = key
        min_match_start_index = match.start()
    if min_match_key:
      min_match = match_dict[min_match_key]
      index = min_match.end()
      if min_match_key.startswith("str"):  #匹配到str开头的match_pattern
        pure_str = min_match.group(1)
        translation_id_list.append(pure_str)
        # print(pure_str)
        # print("===========================")
      # print(content[min_match.start():min_match.end()])
      # print("=================================")
    else:
      break
  return translation_id_list



def CollectPythonExcelTranslationIds(translation_id_set, xx_string_file_path):
  line_list = ExcelUtil.ReadExcelAsLineListFromFilePath(xx_string_file_path)
  for line in line_list:
    translation_id_set.add(line[0])


#将translation_ids导出到export_translation_file_path中
def ExportTranslationIds(translation_ids):
  data_start_row = ExportXlsxConst.Sheet_Data_Start_Row # 数据开始的行号
  workbook = load_workbook(TranslationConst.Export_Translation_File_Path, data_only=True)#data_only,读取公式的结果，而不是公式本身
  sheet = workbook.worksheets[0]
  ExcelUtil.ClearExcelEmptyRows(sheet,data_start_row)
  row = sheet.max_row + 1
  org_line_list = ExcelUtil.ReadExcelAsLineList(sheet, data_start_row)

  translation_dict = {}
  for line in org_line_list:
    translation_id = line[0]
    translation_dict[translation_id] = True

  for translation_id in translation_ids:
    if translation_id in translation_dict:
      continue
    sheet.cell(row, 1).value = translation_id
    translation_dict[translation_id] = True
    row += 1
  workbook.save(TranslationConst.Export_Translation_File_Path)

def main():
  translation_id_set = CollectTranslationIds()
  ExportTranslationIds(translation_id_set)
  os.system("explorer /select, " + TranslationConst.Export_Translation_File_Path)
  print("finish")
main()
