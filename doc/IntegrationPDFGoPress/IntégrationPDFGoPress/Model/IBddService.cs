using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APMDB.Data;
using SourcesDB.Data;

namespace IntégrationPDFGoPress.Model
{
  

    public interface IBddService : IDisposable
    {
        void DetachAll();
        List<Media> GetAllMediaWithStringFilter(string filter);
        List<Media> GetAllChildByPublication(Media media);
        bool IsPublicatinChild(Media media, ref Media master);
        List<Media> GetAllPublicationGoPress();
        bool SaveOrUpdatePublication(Media media, List<Media> Child, int masterOrChildOrNothing);
        bool DeleteMapping(Media media, List<Media> Child);

        bool SaveOrUpdateParameter(string DirIn, string DirOut, string TimeStart, string TimeEnd, string TimeRestart,
            string DayDeleteRecord);



    }
}
