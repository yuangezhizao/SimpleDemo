using System;
using System.Text.RegularExpressions;

namespace BLL.Sprider.Comments
{
    static  class CommentBase
    {
         /// <summary>
         /// 过滤一些词 重复出现的无用的词
         /// </summary>
         /// <param name="str"></param>
         /// <returns></returns>
         private static string FilterWord(string str)
         {
             str = Regex.Replace(str, "\\&[a-zA-Z]{3,10};", "", RegexOptions.IgnoreCase);//屏蔽已 “&”符号开始 “;”结尾的 信息，
             str = Regex.Replace(str, "[\\d]{6,}", "", RegexOptions.IgnoreCase);//屏蔽6位以上的数字
             str = str.Replace("\n", "").Replace("\t", "").Replace("\r", "");

             string fuhao = Regex.Replace(str, "\\w", "", RegexOptions.IgnoreCase);
             if (str.Length < fuhao.Length * 2)  //非文字的内容过多进行屏蔽
             {
                 return "";
             }

             //屏蔽连续重复出现3次的替换成一次
             int inum = str.Length / 3;
             for (int i = 1; i <= inum; i++)
             {
                 for (int j = 0; j < str.Length - i * 3 + 1; j++)
                 {
                     string f1 = str.Substring(j, i);
                     string f2 = str.Substring(j + 1 * i, i);
                     string f3 = str.Substring(j + 2 * i, i);
                     if (f1 == f2 && f2 == f3)
                     {
                         str = str.Replace(f1 + f1 + f1, f1);
                         if (j > 0)
                         { j--; }
                     }
                 }
             }


             return str;
         }

         private const string Skey = "京东|库巴|送货|发货|价格|快递|苏宁|易购|新蛋|新七天|易迅|国美|当当|亚马逊|卓越|淘宝|红孩子|1号店";


         /// <summary>
         /// 评论内容过滤  1删除包含禁用词 2 删除非文字内容过多 3长度小于minlen的评论 3过滤重复出现的词 
         /// </summary>
         /// <param name="content">评论内容</param>
         /// <param name="minLen">中文长度,包含中文字的个数</param>
         /// <returns></returns>
         public static string FiterContent(string content, int minLen)
         {
             if (string.IsNullOrEmpty(content)||Regex.IsMatch(content, Skey))
                 return "";
             content = FilterWord(content);
             return ChineseWordsLength(content) < minLen ? "" : content;
         }

         /// <summary>
         /// 字符串中包含中文的个数
         /// </summary>
         /// <param name="content"></param>
         /// <returns></returns>
         private static int ChineseWordsLength(string content)
         {
             var result = Regex.Matches(content, "[\u4e00-\u9fa5]");
             return result.Count;
         }
    

    }
}
