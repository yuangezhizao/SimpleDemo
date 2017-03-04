using System;
using ServiceStack.DataAnnotations;

namespace Mode
{
    public class MedicineInfo
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        ///     审批编号
        /// </summary>
        public string ApprovalNum { get; set; }

        public string ProName { get; set; }

        public string EnglishName { get; set; }

        public string Category { get; set; }

        /// <summary>
        ///     剂型
        /// </summary>
        public string Dosageforms { get; set; }

        /// <summary>
        ///     规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        ///     生产单位
        /// </summary>
        public string ProductionUnit { get; set; }

        /// <summary>
        ///     生产地址
        /// </summary>
        public string ProductionAddress { get; set; }

        /// <summary>
        ///     批准日期
        /// </summary>
        public DateTime ApprovalDate { get; set; }

        /// <summary>
        ///     药品本位码
        /// </summary>
        public string DrugbasedCode { get; set; }

        /// <summary>
        ///     药品本位码备注
        /// </summary>
        public string DrugbasedBack { get; set; }

        /// <summary>
        ///     备注
        /// </summary>
        public string Remark { get; set; }
    }
}