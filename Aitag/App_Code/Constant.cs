using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniteErp
{
    public abstract class Constant
    {
        public const String APP_DATA_URL = @"/iMedia_Data";

        public const String ROLE_USER = @"A00";
        public const String ROLE_ADM = @"A01";
        public const String ROLE_MANAGER = @"A02";
        public const String ROLE_EDITOR = @"A03";

        public const int SCALE_WIDTH = 800;
        
        public const int THUMB_HEIGHT = 100;
        public const int THUMB_WIDTH = 100;

        //public static readonly String DOC_DATA_URL = String.Format(@"{0}/Mdfile", APP_DATA_URL);
        public static readonly String DOC_DATA_URL = @"/Mdfile";
        public static readonly String DOC_FILE_URL = String.Format(@"{0}/doc_file", DOC_DATA_URL);
        public static readonly String DOC_SCALE_URL = String.Format(@"{0}/doc_scale", DOC_DATA_URL);
        public static readonly String DOC_TAG_URL = String.Format(@"{0}/doc_tag", DOC_DATA_URL);
        public static readonly String TAG_CROP_URL = String.Format(@"{0}/tag_crop", DOC_DATA_URL);
        public static readonly String TAG_SCALE_URL = String.Format(@"{0}/tag_scale", DOC_DATA_URL);
    }
}