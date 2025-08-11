using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym.Models
{
    public class RoomModel
    {
        public JArray fnGetRoom(JObject model)
        {
            var list = new JArray();
            try
            {
                var room_code = model["ROOM_CODE"]?.ToString() ?? "";
                var room_name = model["ROOM_NAME"]?.ToString() ?? "";
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    list = JArray.FromObject(db.ROOMs
                        .Where(x => (x.ROOM_CODE == room_code || room_code == "") &&
                        (x.ROOM_NAME == room_name || room_name == "")).
                        Select(s => new { 
                            s.ROOM_ID,
                            s.ROOM_CODE,
                            s.ROOM_NAME,
                            s.CAPACITY,
                            s.LOCATION,
                            s.STATUS,
                            s.NOTE
                        }).ToList());

                }
            }
            catch { }
            return list;
        }

        public JObject fnPostRoom(JObject model)
        {
            var result = new JObject();
            result["ErrCode"] = "0";
            result["ErrMsg"] = "";
            result["ErrBack"] = "0";
            try
            {
                var action = model["ACTION"]?.ToString() ?? "";
                var room_id = model["ROOM_ID"]?.ToObject<int>() ?? 0;
                var room_code = model["ROOM_CODE"]?.ToString() ?? "";
                var room_name = model["ROOM_NAME"]?.ToString() ?? "";
                var room_capacity = model["CAPACITY"]?.ToObject<int>() ?? 0;
                var room_location = model["LOCATION"]?.ToString() ?? "";
                var room_note = model["NOTE"]?.ToString() ?? "";
                var room_status = model["STATUS"]?.ToString() ?? "";

                using (var db = ConnectionModel.GymShopDataContext())
                {
                    if(action == "INSERT")
                    {
                        var room = new ROOM();
                        room.ROOM_CODE = room_code;
                        room.ROOM_NAME = room_name;
                        room.CAPACITY = room_capacity;
                        room.LOCATION = room_location;
                        room.STATUS = room_status;
                        room.NOTE = room_note;

                        db.ROOMs.InsertOnSubmit(room);
                    }
                    else if (action == "UPDATE")
                    {
                        var room = db.ROOMs.Where(x => x.ROOM_ID == room_id).FirstOrDefault();
                        room.ROOM_NAME = room_name;
                        room.CAPACITY = room_capacity;
                        room.LOCATION = room_location;
                        room.STATUS = room_status;
                        room.NOTE = room_note;
                    }
                    else
                    {
                        var room = db.ROOMs.Where(x => x.ROOM_ID == room_id).FirstOrDefault();
                        db.ROOMs.DeleteOnSubmit(room);
                    }
                    db.SubmitChanges();
                    result["ErrCode"] = "1";
                    result["ErrMsg"] = $"Success";
                    result["ErrBack"] = $"{room_code}";
                }
            }
            catch(Exception e)
            {
                result["ErrCode"] = "0";
                result["ErrMsg"] = $"{e.ToString()}";
                result["ErrBack"] = "0";
            }
            return result;
        }
    }
}