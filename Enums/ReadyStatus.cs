using System;
using System.ComponentModel.DataAnnotations;

namespace TheBlogFinalMVC.Enums
{
    public enum ReadyStatus
    {
        Incomplete,
        [Display(Name = "Production Ready")]
        ProductionReady,
        [Display(Name = "Preview Ready")]
        PreviewReady
    }
}
