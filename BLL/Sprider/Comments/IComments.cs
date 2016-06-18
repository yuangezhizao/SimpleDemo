using System;
using System.Collections.Generic;
using Mode;

namespace BLL.Sprider.Comments
{
  public  interface IComments
  {
      //string GetCommentUrl(string itemId, int pageid);
      string SiteName { get; set; }
      List<CommentInfo> GetCommentsFirstPage(string itemUrl);
      List<CommentInfo> GetAllComments(string itemUrl);
  }
}
