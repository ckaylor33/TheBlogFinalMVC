using System.ComponentModel;

namespace TheBlogFinalMVC.Enums
{
    public enum ModerationType
    {
        [Description("Political propaganda")]
        Political,
        [Description("Offensive language")]
        Language,
        [Description("Drug references")]
        Drugs,
        [Description("Sexual content")]
        Sexual,
        [Description("Threatening speech")]
        Threatening,
        [Description("Hate speech")]
        HateSpeech,
        [Description("Targeted shaming")]
        Shaming,
        [Description("Spam comments")]
        Spamming,
    }
}
