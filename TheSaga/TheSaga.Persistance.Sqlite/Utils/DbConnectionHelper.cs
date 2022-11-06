using Dapper;
using Fasterflect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TheSaga.Persistance.Sqlite.Utils
{
    public static class DbConnectionHelper
    {
        public static Object Save(
            this IDbConnection Connection,
            String TableName,
            Object Item,
            String PrimaryKey,
            Boolean IsInsert,
            String PostfixSql = null,
            Boolean OverridePrimaryKey = false)
        {
            String finalQuery = SqlGenerateSave(Connection, TableName, Item, PrimaryKey, IsInsert, PostfixSql, OverridePrimaryKey);

            if (finalQuery == null)
                return null;

            try
            {
                return Connection.ExecuteScalar<Object>(finalQuery, Item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static String SqlGenerateSave(
            this IDbConnection Connection,
            String TableName,
            Object Item,
            String PrimaryKey,
            Boolean IsInsert,
            String PostfixSql,
            Boolean OverridePrimaryKey = false)
        {
            if (Connection != null)
            {
                Object lPkValue = null;
                var lColumns = Item.GetType().
                    GetProperties(BindingFlags.Public | BindingFlags.Instance).
                    Select(i => new DbDataColumn()
                    {
                        Name = i.Name

                    }).ToList(); // DatabaseHelper.GetColumns(Connection, TableName);
                var lQuery = new StringBuilder();
                var lValues = new List<DbDataColumnValue>();
                //var lColumnsInQuery = new String[0]; // Columns == null ? new String[0] : Columns.SelectMany(i => i.Split(new char[] { ' ', ';', ',' })).Where(i => i != null && i.Trim() != String.Empty).Select(i => i.Trim()).ToArray();
                var lAllColumns = true; // lColumnsInQuery.Length == 0 ? true : false;
                // ObjectValuesHelper.GetProperties
                // pobranie klucza głównego

                var lPrimaryKey = PrimaryKey != null ?
                    RefUnsensitiveHelper.I.GetGetter(Item, PrimaryKey) :
                    null;

                lPkValue = lPrimaryKey != null ? lPrimaryKey(Item) : null;

                for (int i = 0; i < lColumns.Count; i++)
                {
                    var lColumn = lColumns[i];

                    // iteracja do propertiesach
                    var lProperty = RefUnsensitiveHelper.I.GetGetter(Item, lColumn.Name); // ReflectionValueHelper.GetProperty(Item, lColumn.Name);
                    if (lProperty != null)
                    {
                        if (lAllColumns/* ||
                            lColumnsInQuery.FirstOrDefault(col => col.EqualsNonsensitive(lColumn.Name)) != null*/)
                        {
                            lValues.Add(new DbDataColumnValue()
                            {
                                // IsDateTime = lColumn.IsDateTime,
                                Name = lColumn.Name,
                                Value = lProperty(Item) // .Value
                            });
                        }
                    }
                }

                if (lPkValue != null)
                    if (lPkValue != null)
                        OverridePrimaryKey = true;

                if (lValues.Count > 0) // && lPkValue != null)
                {
                    if (IsInsert)
                    {
                        lQuery.
                            Append(" insert into " + TableName + " ( ");

                        var lCount = 0;
                        foreach (var lValue in lValues)
                        {
                            // dla sqlite
                            if (OverridePrimaryKey || !lValue.Name.EqualsNonsensitive(PrimaryKey))
                            {
                                if (lCount > 0) lQuery.Append(", ");
                                lQuery.Append(lValue.Name);
                                lCount++;
                            }
                        }

                        lQuery.
                            Append(" ) values ( ");

                        lCount = 0;
                        foreach (var lValue in lValues)
                        {
                            if (OverridePrimaryKey || !lValue.Name.EqualsNonsensitive(PrimaryKey))
                            {
                                if (lCount > 0) lQuery.Append(", ");
                                lQuery.Append($"@{lValue.Name}");
                                lCount++;
                            }
                            // dla sqlite
                            /*else
                            {
                                if (lCount > 0) lQuery.Append(", ");
                                lQuery.AppendVal(null);
                                lCount++;
                            }*/
                        }

                        lQuery.
                            Append(" ) ");
                    }
                    else
                    {
                        lQuery.Append(" update " + TableName + " set ");

                        var lCount = 0;
                        for (int i = 0; i < lValues.Count; i++)
                        {
                            var lValue = lValues[i];
                            if (!lValue.Name.EqualsNonsensitive(PrimaryKey))
                            {
                                if (lCount > 0) lQuery.Append(", ");
                                lQuery.Append(lValue.Name).Append($" = @{lValue.Name}");
                                lCount++;
                            }
                        }

                        lQuery.Append(" where ").Append(PrimaryKey).Append($" = @{PrimaryKey}");
                    }

                    return lQuery.ToString() + (PostfixSql ?? "");
                }
                else
                {
                    throw new Exception("Nie można wykonać update!");
                }
            }
            return null;
        }
    }

    public class DbDataColumnValue
    {
        public String Name;

        public Boolean IsDateTime;

        public Object Value;
    }

    public static class ObjecrExtensions
    {
        public static Boolean EqualsNonsensitive(this String Str1, String Str2)
        {
            if (Str1 != null && Str2 == null) return false;
            else if (Str1 == null && Str2 != null) return false;
            else if (Str1 == null && Str2 == null) return true;
            else
            {
                return Str2.ToLower().Equals(Str1.ToLower());
            }
        }

    }
    public static class RefUnsensitiveHelper
    {
        private static RefHelperBase _i;

        private static Object lck = new Object();

        public static RefHelperBase I
        {
            get
            {
                if (_i == null)
                {
                    lock (lck)
                    {
                        if (_i == null)
                        {
                            _i = new RefHelperBase(true);
                        }
                    }
                }
                return _i;
            }
        }
    }
    public class RefHelperBase
    {
        private Dictionary<Type, Dictionary<String, MemberSetter>> _cacheSetter =
            new Dictionary<Type, Dictionary<String, MemberSetter>>();

        private Dictionary<Type, Dictionary<String, MemberGetter>> _cacheGetter =
            new Dictionary<Type, Dictionary<String, MemberGetter>>();

        private Dictionary<Type, String[]> _cacheProperties =
            new Dictionary<Type, String[]>();

        private Dictionary<Type, Dictionary<String, Type>> _cachePropertiesTypes =
            new Dictionary<Type, Dictionary<String, Type>>();

        private object lck = new object();

        private bool unsensitive;

        //////////////////////////////////////////////////

        public RefHelperBase(bool Unsensitive)
        {
            this.unsensitive = Unsensitive;
        }

        //////////////////////////////////////////////////

        public Object GetValue(Object Item, String PropertyName)
        {
            var getter = GetGetter(Item, PropertyName);
            if (getter != null)
                return getter(Item);
            return null;
        }

        public DataType GetValue<DataType>(Object Item, String PropertyName)
        {
            var getter = GetGetter(Item, PropertyName);
            if (getter != null)
                return (DataType)getter(Item);
            return default(DataType);
        }

        ////////////////////////////////////////////

        public Type GetPropertyType(Object Object, String Name)
        {
            return Object != null ?
                GetPropertyType(Object.GetType(), Name) :
                null;
        }

        public Type GetPropertyType(Type Type, String Name)
        {
            if (this.unsensitive) Name = Name.ToUpper();

            if (!_cachePropertiesTypes.ContainsKey(Type))
            {
                lock (lck)
                {
                    if (!_cachePropertiesTypes.ContainsKey(Type))
                    {
                        _cachePropertiesTypes[Type] = Type.GetProperties().ToDictionary(
                            p => this.unsensitive ? p.Name.ToUpper() : p.Name,
                            p => p.PropertyType);
                    }
                }
            }
            return _cachePropertiesTypes.ContainsKey(Type) && _cachePropertiesTypes[Type].ContainsKey(Name) ?
                _cachePropertiesTypes[Type][Name] :
                null;
        }

        public String[] GetProperties(Object Object)
        {
            return Object != null ?
                GetProperties(Object.GetType()) :
                new String[0];
        }

        public String[] GetProperties(Type Type)
        {
            if (!_cacheProperties.ContainsKey(Type))
            {
                lock (lck)
                {
                    if (!_cacheProperties.ContainsKey(Type))
                    {
                        _cacheProperties[Type] = Type.GetProperties().Select(p => this.unsensitive ? p.Name.ToUpper() : p.Name).ToArray();
                    }
                }
            }
            return _cacheProperties.ContainsKey(Type) ?
                _cacheProperties[Type] :
                new String[0];
        }


        public MemberSetter GetSetter(Object Object, String Name)
        {
            if (Object != null)
                return GetSetter(Object.GetType(), Name);
            return null;
        }

        public MemberSetter GetSetter(Type type, String Name)
        {
            if (type != null && !String.IsNullOrEmpty(Name))
            {
                if (this.unsensitive) Name = Name.ToUpper();

                Dictionary<String, MemberSetter> innerDict = null;

                if (!_cacheSetter.ContainsKey(type))
                {
                    lock (lck)
                    {
                        if (!_cacheSetter.ContainsKey(type))
                        {
                            _cacheSetter[type] = innerDict = new Dictionary<String, MemberSetter>();
                        }
                    }
                }
                innerDict = _cacheSetter[type];

                if (!innerDict.ContainsKey(Name))
                {
                    lock (lck)
                    {
                        if (!innerDict.ContainsKey(Name))
                        {
                            MemberSetter setter = null;
                            PropertyInfo property = this.unsensitive ? type.GetProperties().FirstOrDefault(p => p.Name.ToUpper().Equals(Name)) : type.GetProperty(Name);
                            FieldInfo field = this.unsensitive ? type.GetFields().FirstOrDefault(p => p.Name.ToUpper().Equals(Name)) : type.GetField(Name);

                            if (property != null || field != null)
                            {
                                if (property != null)
                                    setter = type.DelegateForSetPropertyValue(property.Name);

                                if (setter == null)
                                    if (field != null)
                                        setter = type.DelegateForSetFieldValue(field.Name);
                            }

                            innerDict[Name] = setter;
                        }
                    }
                }

                return innerDict.ContainsKey(Name) ? innerDict[Name] : null;
            }
            return null;
        }

        public MemberGetter GetGetter(Object Object, String Name)
        {
            if (Object != null)
                return GetGetter(Object.GetType(), Name);
            return null;
        }

        public MemberGetter GetGetter(Type type, String Name)
        {
            if (type != null && !String.IsNullOrEmpty(Name))
            {
                if (this.unsensitive) Name = Name.ToUpper();

                Dictionary<String, MemberGetter> innerDict = null;

                lock (lck)
                {
                    if (!_cacheGetter.ContainsKey(type))
                    {
                        if (!_cacheGetter.ContainsKey(type))
                        {
                            _cacheGetter[type] = innerDict = new Dictionary<String, MemberGetter>();
                        }
                    }
                }
                innerDict = _cacheGetter[type];

                lock (lck)
                {
                    if (!innerDict.ContainsKey(Name))
                    {
                        if (!innerDict.ContainsKey(Name))
                        {
                            MemberGetter getter = null;
                            PropertyInfo property = this.unsensitive ? type.GetProperties().FirstOrDefault(p => p.Name.ToUpper().Equals(Name)) : type.GetProperty(Name);
                            FieldInfo field = this.unsensitive ? type.GetFields().FirstOrDefault(p => p.Name.ToUpper().Equals(Name)) : type.GetField(Name);

                            if (property != null || field != null)
                            {
                                if (property != null)
                                    getter = type.DelegateForGetPropertyValue(property.Name);

                                if (getter == null)
                                    if (field != null)
                                        getter = type.DelegateForGetFieldValue(field.Name);
                            }

                            innerDict[Name] = getter;
                        }
                    }
                }

                return innerDict.ContainsKey(Name) ? innerDict[Name] : null;
            }
            return null;
        }
    }


    public class DbDataColumn
    {
        public String Name;

        // public Boolean IsDateTime;

        // public String Default;

        //public Boolean IsNull;

        // public String ValueForNull;
    }
}
