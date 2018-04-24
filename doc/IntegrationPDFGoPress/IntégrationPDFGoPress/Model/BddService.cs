using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using APMDB.Data;

using SourcesDB.Data;
using IntégrationPDFGoPress.Utils;

namespace IntégrationPDFGoPress.Model
{
    public class BddService : IBddService
    {

        private readonly SourcesDBEntities _bdd;
        private readonly APMDBEntities _bddAMPDB;


        public BddService()
        {

            _bdd = new SourcesDBEntities();
            _bddAMPDB = new APMDBEntities();
        }
        public void DetachAll()
        {
            try
            {
                foreach (DbEntityEntry entityEntry in this._bdd.ChangeTracker.Entries().ToArray())
                {

                    if (entityEntry.Entity != null)
                    {
                        entityEntry.State = EntityState.Detached;
                    }
                }
                Logger.LogDebug("Détacher tous les objets du le context");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

        }
        public List<Media> GetAllMediaWithStringFilter(string filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    string sql = @"select * from Media where MediaName like '%" + filter + "%'  and GoPress = 1 and Active = 1 order by MediaName asc";
                    var temp = _bdd.Database.SqlQuery<Media>(sql).ToList();
                    return temp;
                }
                else return _bdd.Media.Where(x => x.Active == true & x.GoPress == true).OrderBy(w => w.MediaName).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return null;
            }
        }

        public List<Media> GetAllChildByPublication(Media media)
        {
            try
            {
                List<MediaMasterChild> childMaster =
                    _bdd.MediaMasterChild.Where(x => x.MediaID == media.MediaID).ToList();
                List<Media> res = new List<Media>();
                childMaster.ForEach(delegate(MediaMasterChild pub)
                {
                    var temp = _bdd.Media.Where(x => x.MediaID == pub.MediaIDChild).FirstOrDefault();
                    if(temp != null)
                        res.Add(temp);
                });
                return res;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return null;
            }
        }

        public bool IsPublicatinChild(Media media,ref Media master)
        {
            try
            {
                MediaMasterChild Child =
                    _bdd.MediaMasterChild.Where(x => x.MediaIDChild == media.MediaID).FirstOrDefault();
                if (Child != null)
                {
                    Media masters = _bdd.Media.Where(x => x.MediaID == Child.MediaID).FirstOrDefault();
                    if (masters != null)
                    {
                        master = masters;
                        return true;
                    }
                }
                else return false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return false;
            }
        }

        public List<Media> GetAllPublicationGoPress()
        {
            try
            {
                List<MediaMasterChild> ChildMaster = _bdd.MediaMasterChild.ToList();
                List<Media> publication = _bdd.Media.Where(x => x.PublicationMaster == false && x.GoPress == true && x.Active == true).ToList();
                ChildMaster.ForEach(delegate(MediaMasterChild mediaMasterChild)
                {
                    publication.Remove(
                        publication.Where(x => x.MediaID == mediaMasterChild.MediaIDChild).FirstOrDefault());
                });
                return publication;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return null;
            }
        }

        public bool SaveOrUpdatePublication(Media media,List<Media> Child, int masterOrChildOrNothing  )
        {
            try
            {
                Media mediaTemp = _bdd.Media.Where(x => x.MediaID == media.MediaID).FirstOrDefault();
                if(mediaTemp != null)
                {
                    mediaTemp.FolderGoPress = media.FolderGoPress;
                    mediaTemp.PublicationMaster = media.PublicationMaster;
                }
                if (masterOrChildOrNothing == 0 && Child != null)
                {
                    List<MediaMasterChild> OldChild =
                        _bdd.MediaMasterChild.Where(x => x.MediaID == media.MediaID).ToList();
                    foreach (var newChild in Child)
                    {
                        var temp = OldChild.Where(x => x.MediaIDChild == newChild.MediaID).FirstOrDefault();
                        if (temp == null)
                        {
                            _bdd.MediaMasterChild.Add(new MediaMasterChild() {MediaID = media.MediaID,MediaIDChild = newChild.MediaID});
                        }
                    }
                    foreach (var old in OldChild)
                    {
                        var temp = Child.Where(x => x.MediaID == old.MediaIDChild).FirstOrDefault();
                        if (temp == null)
                        {
                            _bdd.MediaMasterChild.Remove(old);
                        }
                    }
                }
                if (masterOrChildOrNothing == 1)
                {
                    MediaMasterChild ChildDelete =
                        _bdd.MediaMasterChild.Where(x => x.MediaIDChild == media.MediaID).FirstOrDefault();
                    if (ChildDelete != null)
                    {
                        _bdd.MediaMasterChild.Remove(ChildDelete);
                    }
                    else
                    {
                        Logger.LogError("Impossible de trouver la publication " + media.MediaName +
                                        " en tant qu'enfant ");
                        return false;
                    }
                    foreach (var chd in Child)
                    {
                        _bdd.MediaMasterChild.Add(new MediaMasterChild() {MediaID = media.MediaID,MediaIDChild = chd.MediaID});
                    }   

                }
                if (masterOrChildOrNothing == 2)
                {
                    foreach (var chd in Child)
                    {
                        _bdd.MediaMasterChild.Add(new MediaMasterChild() { MediaID = media.MediaID, MediaIDChild = chd.MediaID });
                    }
                }
                if (masterOrChildOrNothing == 3)
                {
                    List<MediaMasterChild> listChild =
                        _bdd.MediaMasterChild.Where(x => x.MediaID == media.MediaID).ToList();
                    if (listChild.Count > 0)
                    {
                        _bdd.MediaMasterChild.RemoveRange(listChild);
                    }
                }
                General.SaveContextChanges("Update de la publication "+media.MediaName , _bdd);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return false;
            }
        }

        public bool DeleteMapping(Media media, List<Media> Child)
        {
            try
            {
                if (Child != null && Child.Count > 0)
                {
                    List<MediaMasterChild> listChild =
                        _bdd.MediaMasterChild.Where(x => x.MediaID == media.MediaID).ToList();
                    if (listChild.Count > 0)
                    {
                        _bdd.MediaMasterChild.RemoveRange(listChild);
                    }
                }
                Media mediaTemp = _bdd.Media.Where(x => x.MediaID == media.MediaID).FirstOrDefault();
                if (mediaTemp != null)
                {
                    mediaTemp.FolderGoPress = null;
                    mediaTemp.PublicationMaster = false;
                }
                General.SaveContextChanges("delete mapping publication " + media.MediaName, _bdd);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.Message);
                return false;
            }
        }

        public bool SaveOrUpdateParameter(string DirIn,string DirOut, string TimeStart,string TimeEnd,string TimeRestart,string DayDeleteRecord)
        {
            try
            {
                var start = _bddAMPDB.Parameters.Where(x => x.ParamName == "StartServicePDFIntegrationGopress").FirstOrDefault();
                var end = _bddAMPDB.Parameters.Where(x => x.ParamName == "EndServicePDFIntegrationGopress").FirstOrDefault();
                var inPut = _bddAMPDB.Parameters.Where(x => x.ParamName == "IntegrationGopressDirIn").FirstOrDefault();
                var ouPut = _bddAMPDB.Parameters.Where(x => x.ParamName == "IntegrationGopressDirOut").FirstOrDefault();
                var reStart = _bddAMPDB.Parameters.Where(x => x.ParamName == "PeriodServicePDFIntegrationGopress").FirstOrDefault();
                var dayDelete = _bddAMPDB.Parameters.Where(x => x.ParamName == "DeletingRecordPDFIntegrationGopress").FirstOrDefault();
                if (start != null)
                    start.ParamValue = TimeStart;
                if (end != null)
                    end.ParamValue = TimeEnd;
                if (inPut != null)
                    inPut.ParamValue = DirIn;
                if (ouPut != null)
                    ouPut.ParamValue = DirOut;
                if (reStart != null)
                    reStart.ParamValue = TimeRestart;
                if (dayDelete != null)
                    dayDelete.ParamValue = DayDeleteRecord;
                General.SaveContextChanges("Save Paramèter", _bddAMPDB);
                return true;
            }
            catch (Exception ex) 
            {
                Logger.LogCritical(ex.Message);
                return false;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        
    }
}
