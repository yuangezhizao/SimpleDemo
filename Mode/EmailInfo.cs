namespace Mode
{
    public class EmailInfo
    {
        public string Body { get; set; }

        public string Subject { get; set; }
        /// <summary>
        /// 发件人是否随机
        /// </summary>
        public string RandFrom { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string sendToName { get; set; }
        
    }
}
