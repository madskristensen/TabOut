using System.ComponentModel;

namespace TabOut
{
    //internal partial class OptionsProvider
    //{
    //    // Register the options with this attribute on your package class:
    //    // [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "TabOut", "General", 0, 0, true, SupportsProfiles = true)]
    //    [ComVisible(true)]
    //    public class GeneralOptions : BaseOptionPage<General> { }
    //}

    public class General : BaseOptionModel<General>, IRatingConfig
    {
        [Browsable(false)]
        public int RatingRequests { get; set; }
    }
}
