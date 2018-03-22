using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCDN_Client
{
    class DirectoryFromServer
    {
        private String nameDirectory;
        private String dateCreate;
        private String id;
        //private String directLink;

        public string NameDirectory
        {
            get
            {
                return nameDirectory;
            }

            set
            {
                nameDirectory = value;
            }
        }

        public string DateCreate
        {
            get
            {
                return dateCreate;
            }

            set
            {
                dateCreate = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        //public string DirectLink
        //{
        //    get
        //    {
        //        return directLink;
        //    }

        //    set
        //    {
        //        directLink = value;
        //    }
        //}
    }
}
