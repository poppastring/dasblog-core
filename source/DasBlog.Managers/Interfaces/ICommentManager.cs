using System.Collections.Generic;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
    public interface ICommentManager
    {
        CommentSaveState AddComment(string postid, Comment comment);
        CommentSaveState DeleteComment(string postid, string commentid);
        CommentSaveState ApproveComment(string postid, string commentid);
        CommentCollection GetComments(string postid, bool allComments);
        CommentCollection GetAllComments();
        List<Comment> GetCommentsFrontPage();
        List<Comment> GetCommentsForPage(int pageIndex);
    }
}
