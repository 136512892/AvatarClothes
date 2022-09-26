using Newtonsoft.Json;

namespace ReadyPlayerMe
{
    public class AvatarMetaData
    {
        public readonly int MetaDataVersion = 1;
        public BodyType BodyType;
        public OutfitGender OutfitGender;
        public int OutfitVersion;

        public bool IsOutfitMasculine()
        { 
            return OutfitGender == OutfitGender.Masculine;
        }

        public bool IsFullbody()
        { 
            return BodyType == BodyType.Fullbody;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
