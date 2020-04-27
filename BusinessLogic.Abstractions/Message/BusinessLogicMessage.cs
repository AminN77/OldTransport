using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BusinessLogic.Abstractions.Message
{
    public abstract class BusinessLogicMessageBase : IPresentationMessage
    {
        public MessageType Type { get; }

        public string ViewMessage { get; }

        public int MessageCode { get; }

        internal BusinessLogicMessageBase(MessageType type, string viewMessage, int messageCode)
        {
            Type = type;
            ViewMessage = viewMessage;
            MessageCode = messageCode;
        }
    }

    public class BusinessLogicMessage : BusinessLogicMessageBase, IBusinessLogicMessage
    {
        public MessageId Message { get; }

        public BusinessLogicMessage(MessageType type, MessageId message, params object[] viewMessagePlaceHolders)
            : base(type, CreateViewMessage(message, viewMessagePlaceHolders), (int)message)
        {
            Message = message;
        }

        private static string CreateViewMessage(MessageId message, params object[] viewMessagePlaceHolders)
        {
            var viewMessage = message.GetType().GetMember(message.ToString()).First()
                .GetCustomAttribute<DisplayAttribute>()?.GetName();
            if (string.IsNullOrWhiteSpace(viewMessage)) viewMessage = message.ToString();
            if (viewMessagePlaceHolders != null && viewMessagePlaceHolders.Length > 0) viewMessage = string.Format(viewMessage, viewMessagePlaceHolders);
            return viewMessage;
        }
    }
}
