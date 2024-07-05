using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiTest.Models;

[Table("Tbl_comments")]
public partial class TblComment
{
    [Key]
    [Column("comment_id")]
    public int CommentId { get; set; }

    [Column("comment_text", TypeName = "text")]
    public string? CommentText { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("post_id")]
    public int? PostId { get; set; }

    [ForeignKey("PostId")]
    [InverseProperty("TblComments")]
    public virtual TblPost? Post { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("TblComments")]
    public virtual TblUser? User { get; set; }
}
