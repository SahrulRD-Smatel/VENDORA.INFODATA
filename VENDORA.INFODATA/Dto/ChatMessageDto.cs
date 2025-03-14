namespace VENDORA.INFODATA.Dto
{
    public class ChatMessageDto
    {
        public int ThreadId { get; set; }
        public int ReceiverId { get; set; }
        public int? ParentMessageId { get; set; }
        public string Message { get; set; }
        public string FilePath { get; set; }
    }
}
