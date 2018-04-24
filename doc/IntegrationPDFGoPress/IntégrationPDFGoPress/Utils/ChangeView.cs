namespace IntégrationPDFGoPress.Utils
{
    class ChangeView
    {
        public ViewType Viewx { get; set; }
        public ChangeView(ViewType input)
        {
            Viewx = input;
        }
    }

    enum ViewType
    {
        PublicationList,
        Mapping,
        Service
    }
}
