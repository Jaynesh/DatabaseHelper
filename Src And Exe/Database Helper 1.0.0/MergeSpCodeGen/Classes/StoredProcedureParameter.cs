using System;
using System.Collections.Generic;
using System.Text;

namespace GetDatabaseSp
{
    public class StoredProcedureParameter
    {
        string parameterName;
        string parameterType;
        int parameterLength;
        bool isParameterOutput;

        public StoredProcedureParameter() { }
        public StoredProcedureParameter(string parameterName, string parameterType, int parameterLength, bool isParameterOutput)
        {
            this.ParameterName = parameterName;
            this.ParameterType = parameterType;
            this.ParameterLength = parameterLength;
            this.IsParameterOutput = isParameterOutput;
        }

        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }
        
        public string ParameterType
        {
            get { return parameterType; }
            set { parameterType = value; }
        }
        
        public int ParameterLength
        {
            get { return parameterLength; }
            set { parameterLength = value; }
        }
        
        public bool IsParameterOutput
        {
            get { return isParameterOutput; }
            set { isParameterOutput = value; }
        }
    }

    public class StoredProcedureParameterCollection : System.Collections.CollectionBase
    {
        public void Add(StoredProcedureParameter item)
        {
            base.List.Add(item);
        }

        public void Remove(StoredProcedureParameter item)
        {
            base.List.Remove(item);
        }

        public StoredProcedureParameter this[int index]
        {
            get
            {
                return (StoredProcedureParameter)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }
    }
}
