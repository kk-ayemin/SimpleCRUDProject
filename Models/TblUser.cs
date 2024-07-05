using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApiTest.Models;

[Table("Tbl_user")]
public partial class TblUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [StringLength(255)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column("is_active")]
    public bool? IsActive { get; set; }

    [Column("name")]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<TblComment> TblComments { get; set; } = new List<TblComment>();

    [InverseProperty("User")]
    public virtual ICollection<TblPost> TblPosts { get; set; } = new List<TblPost>();
}
