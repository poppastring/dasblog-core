using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
    public interface ICommentManager
    {
        Task<CommentSaveState> AddCommentAsync(string postid, Comment comment, CancellationToken cancellationToken = default);
        CommentSaveState DeleteComment(string postid, string commentid);
        CommentSaveState ApproveComment(string postid, string commentid);
        CommentSaveState UnapproveComment(string postid, string commentid);
        CommentSaveState MarkCommentAsSpam(string postid, string commentid);
        CommentCollection GetComments(string postid, bool allComments);
        CommentCollection GetAllComments();
        List<Comment> GetCommentsFrontPage();
        List<Comment> GetCommentsForPage(int pageIndex);
    }
}
