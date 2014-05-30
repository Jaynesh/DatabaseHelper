/* ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''                                                               
''''    Author:         Jaynesh Shah
''''    Copyright:      © Copyright 2014          
''''    Description:    
''''    Filename:       CodeDomDatabaseSQLDMO.vb                     
''''    Name:           CodeDomDatabaseSQLDMO
''''    Namespace:      
''''
''''                                                                
''''    Version Control                                            
''''    ***************                                             
''''    30-05-2014 :   Initial implementation (v1.0)
''''                                                               
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace CodeDomDatabaseSQLDMO
{
    class Column
    {
        private string colName;        
        private string colType;

        public Column() { }
        public Column(string columnName, string columnType)
        {
            this.ColumnName = columnName;
            this.ColumnType = columnType;
        }

        public string ColumnName
        {
            get { return colName; }
            set { colName = value; }
        }

        public string ColumnType
        {
            get { return colType; }
            set { colType = value; }
        }


    }
}
