using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{


    //  <?xml version="1.0" encoding="utf-8" ?>
    //  <configuration>
    //    <startup>
    //      <supportedRuntime version = "v4.0" sku=".NETFramework,Version=v4.7.2" />
    //    </startup>
    //    <connectionStrings>
    //      <add name = "MyConnection" connectionString="Data Source=127.0.0.1;Initial Catalog=Test;User ID=sa;Password=123456" />
    //    </connectionStrings>
    //  </configuration>

    /// <summary>
    /// SQL数据库帮助类
    /// </summary>
    public class SqlHelper_001
    {
        #region 字段定义

        //定义全局链接
        private SqlConnection _con;

        private SqlTransaction _trans = null;

        private SqlCommand _cmd = null;

        private SqlCommand _withTransCmd = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 对连接的数据库进行初始化操作
        /// </summary>
        /// <param name="connection">链接</param>
        public SqlHelper_001(SqlConnection connection)
        {
            _con = connection;
        }


        /// <summary>
        /// 对连接字段串进行初始化的构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlHelper_001(string connectionString)
        {
            _con = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 默认使用配置文件中MyConnection的连接字符串作为数据库的默认连接
        /// </summary>
        public SqlHelper_001()
        {
            _con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString);
        }

        /// <summary>
        /// 对连接的服务器、数据库、用户名、密码进行初始化
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="database">数据库</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public SqlHelper_001(string server, string database, string username, string password)
        {
            string connectString = string.Format("packet size=4096;server={0};database={1};uid={2};pwd={3}",
                server, database, username, password);
            _con = new SqlConnection(connectString);

        }

        #endregion

        #region 属性

        /// <summary>
        /// 获取服务器名称
        /// </summary>
        public string Server
        {
            get { return _con.DataSource; }
        }

        /// <summary>
        /// 获取数据库名称
        /// </summary>
        public string Database
        {
            get { return _con.Database; }
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public SqlConnection Connection
        {
            get { return _con; }
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取查询的数据表
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <returns>返回得到的数据表</returns>
        public DataTable QueryDataTable(string queryCommandText)
        {
            return QueryDataTable(queryCommandText, false);
        }

        /// <summary>
        /// 获取查询的数据表，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">传入的sql查询语句</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回一个datatable的数据表</returns>
        public DataTable QueryDataTable(string queryCommandText, params SqlParameter[] parameters)
        {
            return QueryDataTable(queryCommandText, false, parameters);
        }

        /// <summary>
        /// 获取查询的数据表，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">传入的sql查询语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回一个datatable的数据表</returns>
        public DataTable QueryDataTable(string queryCommandText, bool isProcedure, params SqlParameter[] parameters)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(queryCommandText, _con);
                cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;

                if (parameters != null && parameters.Count() > 0)
                {
                    ConvertNullToDBNull(parameters);
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                    _con.Close();
            }
        }

        /// <summary>
        /// 获取查询的数据集
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <returns>返回得到的数据集</returns>
        public DataSet QueryDataSet(string queryCommandText)
        {
            return QueryDataSet(queryCommandText, false);
        }

        /// <summary>
        /// 获取查询的数据集，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">传入的sql查询语句</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回得到的数据集</returns>
        public DataSet QueryDataSet(string queryCommandText, params SqlParameter[] parameters)
        {
            return QueryDataSet(queryCommandText, false, parameters);
        }

        /// <summary>
        /// 获取查询的数据集，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">传入的sql查询语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回得到的数据集</returns>
        public DataSet QueryDataSet(string queryCommandText, bool isProcedure, params SqlParameter[] parameters)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(queryCommandText, _con);
                cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                if (parameters != null && parameters.Count() > 0)
                {
                    ConvertNullToDBNull(parameters);
                    cmd.Parameters.AddRange(parameters);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                    _con.Close();
            }
        }

        /// <summary>
        /// 得到查询的数据只读器
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <returns>返回的Reader</returns>
        public SqlDataReader QueryReader(string queryCommandText)
        {
            return QueryReader(queryCommandText, false);
        }

        /// <summary>
        /// 得到查询的数据只读器，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回的Reader</returns>
        public SqlDataReader QueryReader(string queryCommandText, params SqlParameter[] parameters)
        {
            return QueryReader(queryCommandText, false, parameters);
        }

        /// <summary>
        /// 得到查询的数据只读器，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回的Reader</returns>
        public SqlDataReader QueryReader(string queryCommandText, bool isProcedure, params SqlParameter[] parameters)
        {

            SqlCommand cmd = new SqlCommand(queryCommandText, _con);
            cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
            if (parameters != null && parameters.Count() > 0)
            {
                ConvertNullToDBNull(parameters);
                cmd.Parameters.AddRange(parameters);
            }
            if (_con.State == ConnectionState.Closed)
                _con.Open();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);

        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <returns>返回首行首列</returns>
        public object QueryObject(string queryCommandText)
        {
            return QueryObject(queryCommandText, false);
        }

        /// <summary>
        /// 获取首行首列，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回首行首列</returns>
        public object QueryObject(string queryCommandText, params SqlParameter[] parameters)
        {
            return QueryObject(queryCommandText, false, parameters);
        }

        /// <summary>
        /// 获取首行首列，使用参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回首行首列</returns>
        public object QueryObject(string queryCommandText, bool isProcedure, params SqlParameter[] parameters)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(queryCommandText, _con);
                cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                if (parameters != null && parameters.Count() > 0)
                {
                    ConvertNullToDBNull(parameters);
                    cmd.Parameters.AddRange(parameters);
                }

                if (_con.State == ConnectionState.Closed)
                    _con.Open();
                return cmd.ExecuteScalar();
            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                    _con.Close();
            }
        }

        /// <summary>
        /// 将查询的结果返回一个实体的集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <returns>返回结果</returns>
        public List<T> QueryEntityList<T>(string queryCommandText)
        {
            return QueryEntityList<T>(queryCommandText, false);
        }

        /// <summary>
        /// 将查询的结果返回一个实体的集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回结果</returns>
        public List<T> QueryEntityList<T>(string queryCommandText, params SqlParameter[] parameters)
        {
            return QueryEntityList<T>(queryCommandText, false, parameters);
        }

        /// <summary>
        /// 将查询的结果返回一个实体的集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回结果</returns>
        public List<T> QueryEntityList<T>(string queryCommandText, bool isProcedure, params SqlParameter[] parameters)
        {
            try
            {
                Type ty = typeof(T);
                PropertyInfo[] pr = ty.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
                SqlCommand cmd = new SqlCommand(queryCommandText, _con);
                cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                if (parameters != null && parameters.Count() > 0)
                {
                    ConvertNullToDBNull(parameters);
                    cmd.Parameters.AddRange(parameters);
                }
                if (_con.State == ConnectionState.Closed)
                    _con.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                List<T> lst = new List<T>();
                while (dr.Read())
                {
                    T obj = (T)System.Activator.CreateInstance(ty);
                    foreach (var item in pr)
                    {
                        try
                        {
                            if (dr[item.Name] != null && dr[item.Name] != DBNull.Value)
                            {
                                item.SetValue(obj, dr[item.Name], null);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            continue;
                        }
                    }
                    lst.Add(obj);
                }
                dr.Close();
                return lst.Count == 0 ? null : lst;
            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                    _con.Close();
            }
        }

        /// <summary>
        /// 将查询的结果返回单个实体实例
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <returns>返回的实例</returns>
        public T QuerySingleEntity<T>(string queryCommandText)
        {
            return QuerySingleEntity<T>(queryCommandText, false);
        }

        /// <summary>
        /// 将查询的结果返回单个实体实例
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回的实例</returns>
        public T QuerySingleEntity<T>(string queryCommandText, params SqlParameter[] parameters)
        {
            return QuerySingleEntity<T>(queryCommandText, false, parameters);
        }

        /// <summary>
        /// 将查询的结果返回单个实体实例
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回的实例</returns>
        public T QuerySingleEntity<T>(string queryCommandText, bool isProcedure, params SqlParameter[] parameters)
        {
            try
            {
                Type ty = typeof(T);
                PropertyInfo[] pr = ty.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
                SqlCommand cmd = new SqlCommand(queryCommandText, _con);
                cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                if (parameters != null && parameters.Count() > 0)
                {
                    ConvertNullToDBNull(parameters);
                    cmd.Parameters.AddRange(parameters);
                }
                if (_con.State == ConnectionState.Closed)
                    _con.Open();
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (!dr.HasRows)
                {
                    dr.Close();
                    return default(T);
                }
                else
                {
                    dr.Read();
                    T obj = (T)System.Activator.CreateInstance(ty);
                    foreach (var item in pr)
                    {
                        try
                        {
                            if (dr[item.Name] != null && dr[item.Name] != DBNull.Value)
                            {
                                item.SetValue(obj, dr[item.Name], null);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            continue;
                        }
                    }
                    dr.Close();
                    return obj;
                }
            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                    _con.Close();
            }
        }

        #endregion

        #region 对空参数进行dbnull.value的转换

        /// <summary>
        /// 对参数中null或空字符串 参数转换为dbnull
        /// </summary>
        /// <param name="parameters"></param>
        private void ConvertNullToDBNull(SqlParameter[] parameters)
        {
            if (parameters != null && parameters.Count() > 0)
            {
                foreach (SqlParameter item in parameters)
                {
                    if (item.Value == null || string.Empty.Equals(item.Value))
                        item.Value = DBNull.Value;
                }
            }
        }

        #endregion

        #region 单条增删改数据的执行,可执行控制是否使用事务

        /// <summary>
        /// 数据新增/修改/删除,不使用事务
        /// </summary>
        /// <param name="commandText">传入的执行语句,只能是单个插入、修改、删除语句</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string commandText)
        {
            return Execute(commandText, false, false);

        }

        /// <summary>
        /// 数据新增/修改/删除,可自已选择是否使用事务
        /// </summary>
        /// <param name="commandText">传入的执行语句,只能是单个插入、修改、删除语句</param>
        /// <param name="usingTranscation">是否使用事务方式</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string commandText, bool usingTranscation)
        {
            return Execute(commandText, false, usingTranscation);
        }

        /// <summary>
        /// 数据新增/修改/删除,不使用事务
        /// </summary>
        /// <param name="commandText">传入的执行语句,只能是单个插入、修改、删除语句</param>
        /// <param name="parameters">执行语句所须多个参数集</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string commandText, params SqlParameter[] parameters)
        {
            return Execute(commandText, false, false, parameters);
        }

        /// <summary>
        /// 数据新增/修改/删除,可自已选择是否使用事务
        /// </summary>
        /// <param name="commandText">传入的执行语句,只能是单个插入、修改、删除语句</param>
        /// <param name="usingTranscation">是否使用事务方式</param>
        /// <param name="parameters">执行语句所须多个参数集</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string commandText, bool usingTranscation, params SqlParameter[] parameters)
        {
            return Execute(commandText, false, usingTranscation, parameters);
        }

        /// <summary>
        /// 数据新增/修改/删除,可调用存储过程和指定是否使用事务
        /// </summary>
        /// <param name="commandText">传入的执行语句,只能是单个插入、修改、删除语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="usingTranscation">是否使用事务方式</param>
        /// <param name="parameters">执行语句所须多个参数集</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string commandText, bool isProcedure, bool usingTranscation, params SqlParameter[] parameters)
        {
            SqlTransaction tr = null;
            try
            {
                _cmd = new SqlCommand(commandText, _con);
                if (_con.State == ConnectionState.Closed)
                    _con.Open();

                if (usingTranscation)
                {
                    tr = _con.BeginTransaction();
                    _cmd.Transaction = tr;
                }

                _cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                if (parameters != null && parameters.Count() > 0)
                {
                    ConvertNullToDBNull(parameters);
                    _cmd.Parameters.AddRange(parameters);
                }

                int result = _cmd.ExecuteNonQuery();
                if (tr != null)
                    tr.Commit();
                return result;
            }
            catch (System.Exception ex)
            {
                if (tr != null)
                    tr.Rollback();
                throw ex;
            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                    _con.Close();
            }
        }

        /// <summary>
        /// 数据新增/修改/删除,可调用存储过程和指定是否使用事务
        /// </summary>
        /// <param name="commandText">传入的执行语句,只能是单个插入、修改、删除语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="usingTranscation">是否使用事务方式</param>
        /// <param name="commandTimeout">SQL执行操作的超时时间,默认为30秒</param>
        /// <param name="parameters">执行语句所须多个参数集</param>
        /// <returns>返回受影响的行数</returns>
        public int Execute(string commandText, bool isProcedure, bool usingTranscation, uint commandTimeout, params SqlParameter[] parameters)
        {
            SqlTransaction tr = null;
            try
            {
                _cmd = new SqlCommand(commandText, _con);
                if (_con.State == ConnectionState.Closed)
                    _con.Open();
                _cmd.CommandTimeout = Convert.ToInt32(commandTimeout);
                if (usingTranscation)
                {
                    tr = _con.BeginTransaction();
                    _cmd.Transaction = tr;
                }

                _cmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
                if (parameters != null && parameters.Count() > 0)
                {
                    ConvertNullToDBNull(parameters);
                    _cmd.Parameters.AddRange(parameters);
                }

                int result = _cmd.ExecuteNonQuery();
                if (tr != null)
                    tr.Commit();
                return result;
            }
            catch (System.Exception ex)
            {
                if (tr != null)
                    tr.Rollback();
                throw ex;
            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                    _con.Close();
            }
        }

        /// <summary>
        /// 得到执行SQL语句后指定参数项的值
        /// </summary>
        /// <param name="paremeterName">参数名称</param>
        /// <returns>返回的参数值</returns>
        public object GetExecuteParameterValue(string paremeterName)
        {
            if (_cmd != null && _cmd.Parameters.Count > 0 && _cmd.Parameters[paremeterName] != null)
                return _cmd.Parameters[paremeterName].Value;
            else
                return null;
        }

        #endregion

        #region 事务的自行控制，以下每个方法都显示使用了事务,但提交和回滚控制由用户自行选择

        /// <summary>
        /// 获取事务的方法
        /// </summary>
        /// <returns></returns>
        private SqlTransaction GetTranscation()
        {
            if (_trans == null)
            {
                if (_con.State == ConnectionState.Closed)
                    _con.Open();

                _trans = _con.BeginTransaction();
            }

            return _trans;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void TranscationRollback()
        {
            if (_trans != null)
                _trans.Rollback();
            if (_con.State == ConnectionState.Open)
                _con.Close();
        }

        /// <summary>
        /// 事务提交
        /// </summary>
        public void TranscationCommit()
        {
            if (_trans != null)
                _trans.Commit();
            if (_con.State == ConnectionState.Open)
                _con.Close();
        }

        /// <summary>
        /// 执行某个命令，方法执行时强制使用事务,须要调用者手动调用TranscationRollback或TranscationCommit来进行回滚或提交
        /// </summary>
        /// <param name="commandText">传入的执行语句,可为插入、修改、删除语句</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteWithCommonTrans(string commandText)
        {
            return ExecuteWithCommonTrans(commandText, false);
        }

        /// <summary>
        /// 使用带参数化的方式执行命令，方法执行时强制使用事务,方法执行时强制使用事务,须要调用者手动调用TranscationRollback或TranscationCommit来进行回滚或提交
        /// </summary>
        /// <param name="commandText">传入的执行语句,可为插入、修改、删除语句</param>
        /// <param name="parameters">执行语句所须多个参数集</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteWithCommonTrans(string commandText, params SqlParameter[] parameters)
        {
            return ExecuteWithCommonTrans(commandText, false, parameters);
        }

        /// <summary>
        /// 使用带参数化的方式执行命令或存储过程，方法执行时强制使用事务,方法执行时强制使用事务,须要调用者手动调用TranscationRollback或TranscationCommit来进行回滚或提交
        /// </summary>
        /// <param name="commandText">传入的执行语句,只能是单个插入、修改、删除语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">执行语句所须多个参数集</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteWithCommonTrans(string commandText, bool isProcedure, params SqlParameter[] parameters)
        {
            _withTransCmd = new SqlCommand(commandText, _con);
            _withTransCmd.Transaction = GetTranscation();

            _withTransCmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
            if (parameters != null && parameters.Count() > 0)
            {
                ConvertNullToDBNull(parameters);
                _withTransCmd.Parameters.AddRange(parameters);
            }

            return _withTransCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取首行首列，使用共公事务去执行，并支持参数化方式进行查询
        /// </summary>
        /// <param name="queryCommandText">sql查询语句</param>
        /// <param name="isProcedure">是否调用存储过程</param>
        /// <param name="parameters">查询时所须多个参数集</param>
        /// <returns>返回首行首列</returns>
        public object QueryObjectWithCommonTrans(string queryCommandText, bool isProcedure, params SqlParameter[] parameters)
        {

            _withTransCmd = new SqlCommand(queryCommandText, _con);
            _withTransCmd.Transaction = GetTranscation();

            _withTransCmd.CommandType = isProcedure ? CommandType.StoredProcedure : CommandType.Text;
            if (parameters != null && parameters.Count() > 0)
            {
                ConvertNullToDBNull(parameters);
                _withTransCmd.Parameters.AddRange(parameters);
            }

            return _withTransCmd.ExecuteScalar();

        }


        #endregion

        #region 利用数据库中存储过程来进行分页的功能

        /// <summary>
        /// 利用指定的存储过程实现分页功能,总大小输出到count变量中
        /// </summary>
        /// <param name="procedureName">调用的存储过程的名称</param>
        /// <param name="tables">表名,支持多个Join表进行关联</param>
        /// <param name="pk">主键名称,如果该主键和其它表中列名一致,需加上表名.</param>
        /// <param name="sort">排序字段和规则</param>
        /// <param name="startRowIndex">开始行的索引</param>
        /// <param name="pageSize">页显示的记录数</param>
        /// <param name="fields">要列出的字段名称,有重名的需加表名</param>
        /// <param name="filters">筛选条件,即Where语句后的过滤规则</param>
        /// <param name="group">分组规则</param>
        /// <param name="count">输出的总记录数</param>
        /// <returns>返回的查询后的DataReader</returns>
        private SqlDataReader PagingBySpecifyProcedure(string procedureName, string tables, string pk, string sort,
        int startRowIndex, int pageSize, string fields, string filters, string group, out int count)
        {
            List<SqlParameter> lst = new List<SqlParameter>();
            lst.Add(new SqlParameter("@Tables", SqlDbType.VarChar, 1000));
            lst[lst.Count - 1].Value = tables;
            lst.Add(new SqlParameter("@PK", SqlDbType.VarChar, 100));
            lst[lst.Count - 1].Value = pk;
            lst.Add(new SqlParameter("@Sort", SqlDbType.VarChar, 200));
            lst[lst.Count - 1].Value = sort;
            lst.Add(new SqlParameter("@startRowIndex", SqlDbType.Int));
            lst[lst.Count - 1].Value = startRowIndex;
            lst.Add(new SqlParameter("@PageSize", SqlDbType.Int));
            lst[lst.Count - 1].Value = pageSize;
            lst.Add(new SqlParameter("@Fields", SqlDbType.VarChar, 1200));
            lst[lst.Count - 1].Value = fields;
            lst.Add(new SqlParameter("@Filter", SqlDbType.VarChar, 1000));
            lst[lst.Count - 1].Value = filters;
            lst.Add(new SqlParameter("@Group", SqlDbType.VarChar, 1000));
            lst[lst.Count - 1].Value = group;

            SqlDataReader reader = QueryReader(procedureName, true, lst.ToArray());

            count = 0;
            if (reader != null && reader.Read())
            {
                count = (int)reader[0];
                //读取下一个结果集
                reader.NextResult();
            }

            return reader;
        }

        /// <summary>
        /// 调用存储过程p_Paging_Cursor,实现分页
        /// 此方法使用游标形式,性能最佳,建议使用
        /// </summary>
        /// <param name="tables">表名,支持多个Join表进行关联</param>
        /// <param name="pk">主键名称,如果该主键和其它表中列名一致,需加上表名.</param>
        /// <param name="sort">排序字段和规则</param>
        /// <param name="startRowIndex">开始行的索引</param>
        /// <param name="pageSize">页显示的记录数</param>
        /// <param name="fields">要列出的字段名称,有重名的需加表名</param>
        /// <param name="filters">筛选条件,即Where语句后的过滤规则</param>
        /// <param name="group">分组规则</param>
        /// <param name="count">输出的总记录数</param>
        /// <returns>返回的查询后的DataReader</returns>
        public SqlDataReader PagingByCursor(string tables, string pk, string sort,
           int startRowIndex, int pageSize, string fields, string filters, string group, out int count)
        {
            return PagingBySpecifyProcedure("dbo.p_Paging_Cursor", tables, pk, sort, startRowIndex, pageSize, fields, filters, group, out count);
        }

        /// <summary>
        /// 调用存储过程p_Paging_IDS,实现分页
        /// 此方法将查询记录通过临时表设置一个自增ID,再通过自增ID和记录进行Join
        /// 性能方面稍差,建议不使用
        /// </summary>
        /// <param name="tables">表名,支持多个Join表进行关联</param>
        /// <param name="pk">主键名称,如果该主键和其它表中列名一致,需加上表名</param>
        /// <param name="sort">排序字段和规则</param>
        /// <param name="startRowIndex">开始行的索引</param>
        /// <param name="pageSize">页显示的记录数</param>
        /// <param name="fields">要列出的字段名称,有重名的需加表名</param>
        /// <param name="filters">筛选条件,即Where语句后的过滤规则</param>
        /// <param name="group">分组规则</param>
        /// <param name="count">输出的总记录数</param>
        /// <returns>返回的查询后的DataReader</returns>
        public SqlDataReader PagingByIDS(string tables, string pk, string sort,
         int startRowIndex, int pageSize, string fields, string filters, string group, out int count)
        {
            return PagingBySpecifyProcedure("dbo.p_Paging_IDS", tables, pk, sort, startRowIndex, pageSize, fields, filters, group, out count);
        }

        /// <summary>
        /// 调用存储过程p_Paging_RowCount,实现分页
        /// 此方法通过设置两度设置Set RowCount以及取得主排序列的方式来获取数据
        /// 性能方面中上,可使用
        /// </summary>
        /// <param name="tables">表名,支持多个Join表进行关联</param>
        /// <param name="pk">主键名称,如果该主键和其它表中列名一致,需加上表名</param>
        /// <param name="sort">排序字段和规则</param>
        /// <param name="startRowIndex">开始行的索引</param>
        /// <param name="pageSize">页显示的记录数</param>
        /// <param name="fields">要列出的字段名称,有重名的需加表名</param>
        /// <param name="filters">筛选条件,即Where语句后的过滤规则</param>
        /// <param name="group">分组规则</param>
        /// <param name="count">输出的总记录数</param>
        /// <returns>返回的查询后的DataReader</returns>
        public SqlDataReader PagingByRowCount(string tables, string pk, string sort,
            int startRowIndex, int pageSize, string fields, string filters, string group, out int count)
        {
            return PagingBySpecifyProcedure("dbo.p_Paging_RowCount", tables, pk, sort, startRowIndex, pageSize, fields, filters, group, out count);
        }

        /// <summary>
        /// 调用存储过程p_Paging_RowNumer,实现分页,只有SQL2005以上版本才支持
        /// 此方法调用2005以上数据库RowNumber方式去实现
        /// 性能方面中上,可使用
        /// </summary>
        /// <param name="tables">表名,支持多个Join表进行关联</param>
        /// <param name="pk">主键名称,如果该主键和其它表中列名一致,需加上表名</param>
        /// <param name="sort">排序字段和规则</param>
        /// <param name="startRowIndex">开始行的索引</param>
        /// <param name="pageSize">页显示的记录数</param>
        /// <param name="fields">要列出的字段名称,有重名的需加表名</param>
        /// <param name="filters">筛选条件,即Where语句后的过滤规则</param>
        /// <param name="group">分组规则</param>
        /// <param name="count">输出的总记录数</param>
        /// <returns>返回的查询后的DataReader</returns>
        public SqlDataReader PagingByRowNumber(string tables, string pk, string sort,
            int startRowIndex, int pageSize, string fields, string filters, string group, out int count)
        {
            return PagingBySpecifyProcedure("dbo.p_Paging_RowNumber", tables, pk, sort, startRowIndex, pageSize, fields, filters, group, out count);
        }

        #endregion
    }
}
