using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Visitors
{
    public class OnlineVisitor
    {
        [BsonId]
        ///این اتربیوت باعث می شود آیدی از جنس استرینگ باقی بماند
        ///و تبدیل به جی یو آیدی نشود
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public string ClientId { get; set; }
    }
}
