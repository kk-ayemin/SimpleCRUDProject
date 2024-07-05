using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiTest.Models;

[Table("Tbl_posts")]
public partial class TblPost
{
    [Key]
    [Column("post_id")]
    public int PostId { get; set; }

    [Column("post_text", TypeName = "text")]
    public string? PostText { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [InverseProperty("Post")]
    public virtual ICollection<TblComment> TblComments { get; set; } = new List<TblComment>();

    [ForeignKey("UserId")]
    [InverseProperty("TblPosts")]
    public virtual TblUser? User { get; set; }
}
