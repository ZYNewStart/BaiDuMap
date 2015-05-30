
using System.Windows.Controls;

namespace AppClient
{
   public class ImageButton : Button
    {
        private string _mImagepath;

        public string ImgPath
        {
            get { return _mImagepath; }
            set { _mImagepath = value; }
        }
    }
}
