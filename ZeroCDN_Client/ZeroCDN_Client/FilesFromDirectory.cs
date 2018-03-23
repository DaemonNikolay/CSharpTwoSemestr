using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCDN_Client
{
    class FilesFromDirectory
    {
        private String id;
        private String name;
        private String dateCreate;
        private String directoryId;

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

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
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

        public string DirectoryId
        {
            get
            {
                return directoryId;
            }

            set
            {
                directoryId = value;
            }
        }
    }
}
