using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace NibssFileExtraction
{
    public class RequiredCell
    {
        public static RequiredCellSection _Config = ConfigurationManager.GetSection("requiredCell") as RequiredCellSection;
        public static CellElementCollection GetCells()
        {
            return _Config.Cells;
        }
    }
    public class RequiredCellSection : ConfigurationSection
    {
        [ConfigurationProperty("cells")]
        public CellElementCollection Cells
        {
            get { return (CellElementCollection)this["cells"]; }
        }


    }

    [ConfigurationCollection(typeof(CellElement))]
    public class CellElementCollection : ConfigurationElementCollection
    {
        public CellElement this[int index]
        {
            get { return (CellElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new CellElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CellElement)element).CellName;
        }
    }
    public class CellElement : ConfigurationElement
    {
        public CellElement() { }

        [ConfigurationProperty("cellName", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string CellName
        {
            get { return (string)this["cellName"]; }
            set { this["cellName"] = value; }
        }

        [ConfigurationProperty("columnName", DefaultValue = "", IsRequired = true)]
        public string ColumnName
        {
            get { return (string)this["columnName"]; }
            set { this["columnName"] = value; }
        }



    }
}
