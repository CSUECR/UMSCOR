using System.Collections.Generic;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.GoogleMap;
using Senparc.Weixin.MP.Entities.BaiduMap;
using Senparc.Weixin.MP.Helpers;

namespace MorSun.WX.ZYB.Service
{
    public class LocationService
    {
        public IResponseMessageBase GetResponseMessage(RequestMessageLocation requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);

            var markersList = new List<GoogleMapMarkers>();
            markersList.Add(new GoogleMapMarkers()
            {
                X = requestMessage.Location_X,
                Y = requestMessage.Location_Y,
                Color = "red",
                Label = "S",
                Size = GoogleMapMarkerSize.Default,
            });
            var mapSize = "480x600";
            var mapUrl = GoogleMapHelper.GetGoogleStaticMap(19 /*requestMessage.Scale*//*微信和GoogleMap的Scale不一致，这里建议使用固定值*/,
                                                            markersList, mapSize);

            responseMessage.Articles.Add(new Article()
            {
                Description = string.Format("您刚才发送了地理位置信息。Location_X：{0}，Location_Y：{1}，Scale：{2}，标签：{3}",
                              requestMessage.Location_X, requestMessage.Location_Y,
                              requestMessage.Scale, requestMessage.Label),
                PicUrl = mapUrl,
                Title = requestMessage.Label,
                Url = mapUrl
            });

            //var bdmarkersList = new List<BaiduMarkers>();
            //bdmarkersList.Add(new BaiduMarkers()
            //{
            //    Latitude = requestMessage.Location_X,
            //    Longitude = requestMessage.Location_Y,
            //    Color = "red",
            //    Label = "S",
            //    url = "www.bungma.com",
            //});
            //var bdMapUrl = BaiduMapHelper.GetBaiduStaticMap(requestMessage.Location_X, requestMessage.Location_Y, requestMessage.Scale, 1, bdmarkersList, 400, 300);
            
            //responseMessage.Articles.Add(new Article()
            //{
            //    Description = string.Format("您刚才发送了地理位置信息。Location_X：{0}，Location_Y：{1}，Scale：{2}，标签：{3}",
            //                  requestMessage.Location_X, requestMessage.Location_Y,
            //                  requestMessage.Scale, requestMessage.Label),
            //    PicUrl = bdMapUrl,
            //    Title = "定位地点周边地图",
            //    Url = bdMapUrl
            //});
            //responseMessage.Articles.Add(new Article()
            //{
            //    Title = "邦马网",
            //    Description = "邦马网",
            //    PicUrl = "http://www.bungma.com",
            //    Url = "http://www.bungma.com"
            //});

            return responseMessage;
        }
    }
}