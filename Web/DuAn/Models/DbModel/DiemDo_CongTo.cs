namespace DuAn.Models.DbModel
{
   using System;
   using System.Collections.Generic;
   using System.ComponentModel.DataAnnotations;
   using System.ComponentModel.DataAnnotations.Schema;
   using System.Data.Entity.Spatial;
   using System.Windows.Input;

   [Table("DiemDo-CongTo")]
   public partial class DiemDo_CongTo
   {
      [Key]
      public int ID { get; set; }

      [Column(Order = 0)]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public int DiemDoID { get; set; }

      [Column(Order = 1)]
      [DatabaseGenerated(DatabaseGeneratedOption.None)]
      public int CongToID { get; set; }

      public DateTime ThoiGianBatDau { get; set; }

      public DateTime? ThoiGianKetThuc { get; set; }

      public virtual CongTo CongTo { get; set; }

      public virtual DiemDo DiemDo { get; set; }
   }
}
