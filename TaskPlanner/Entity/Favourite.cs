//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskPlanner.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Favourites")]
    public partial class Favourite
    {
        [Key]
        public int FavouriteId { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public bool IsActive { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Project Project { get; set; }
    }
}
