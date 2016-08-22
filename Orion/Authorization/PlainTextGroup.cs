using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IniParser;
using IniParser.Model;
using Orion.Extensions;

namespace Orion.Authorization
{
	/// <summary>
	/// Plain text user group used by the <see cref="PlainTextAccountService"/>.
	/// </summary>
	public class PlainTextGroup : IGroup
	{
		private IniData _iniData;
		private PlainTextAccountService _service;

		private List<IUserAccount> _members;
		private List<IPermission> _permissions;

		/// <inheritdoc/>
		public string Name
		{
			get
			{
				return _iniData.Sections["Group"]["Name"];
			}
			set
			{
				_iniData.Sections["Group"]["Name"] = value;
				Save();
			}
		}

		/// <inheritdoc/>
		public string Description
		{
			get
			{
				return _iniData.Sections["Group"]["Description"];
			}
			set
			{
				_iniData.Sections["Group"]["Description"] = value;
				Save();
			}
		}

		/// <inheritdoc/>
		public IEnumerable<IUserAccount> Members => _members;

		/// <inheritdoc/>
		public IEnumerable<IPermission> Permissions => _permissions;

		/// <summary>
		/// Gets the computed group file path based on the group name.
		/// </summary>
		protected string GroupPath => Path.Combine(PlainTextAccountService.GroupPathPrefix, $"{Name.Slugify()}.ini");

		/// <summary>
		/// Initializes a new instance of the <see cref="PlainTextGroup"/> class.
		/// </summary>
		/// <param name="service">
		/// A reference to the <see cref="PlainTextAccountService"/> which owns this user group.
		/// </param>
		public PlainTextGroup(PlainTextAccountService service)
		{
			this._service = service;
			this._iniData = new IniData();

			this._iniData.Sections.AddSection("Group");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlainTextGroup"/> class with the provided group name, which
		/// will load the group data from disk.
		/// </summary>
		/// <param name="service">
		/// A reference to the <see cref="PlainTextAccountService"/> which owns this user group.
		/// </param>
		/// <param name="groupName">A string containing the group name to load from disk.</param>
		public PlainTextGroup(PlainTextAccountService service, string groupName)
			: this(service)
		{
			Name = groupName;

			using (FileStream fs = new FileStream(GroupPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
			using (StreamReader sr = new StreamReader(fs))
			{
				_iniData = new StreamIniDataParser().ReadData(sr);
			}

			LoadCollections();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlainTextGroup"/> class from the specified I/O stream.
		/// </summary>
		/// <param name="service">
		/// A reference to the <see cref="PlainTextAccountService"/> which owns this user group.
		/// </param>
		/// <param name="stream">The I/O stream to load the <see cref="PlainTextGroup"/> data from.</param>
		public PlainTextGroup(PlainTextAccountService service, Stream stream)
			: this(service)
		{
			using (StreamReader sr = new StreamReader(stream))
			{
				_iniData = new StreamIniDataParser().ReadData(sr);
			}

			LoadCollections();
		}

		private void LoadCollections()
		{
			_members = new List<IUserAccount>();
			foreach (string s in _iniData.Sections["Group"]["Members"].Split(','))
			{
				var userAccount = _service.GetUserAccountOrDefault(s);
				if (userAccount != null)
				{
					_members.Add(userAccount);
				}
			}

			_permissions = new List<IPermission>();
			foreach (string s in _iniData.Sections["Group"]["Permissions"].Split(','))
			{
				var permission = .FromNode(s);
				if (permission != null)
				{
					_permissions.Add(permission);
				}
			}
		}

		/// <inheritdoc/>
		public void AddMember(IUserAccount userAccount)
		{
			if (!_members.Contains(userAccount))
			{
				_members.Add(userAccount);
				_iniData.Sections["Group"]["Members"] = String.Join(",", _members.Select(u => u.AccountName));
				Save();
			}
		}

		/// <inheritdoc/>
		public void RemoveMember(IUserAccount userAccount)
		{
			if (_members.Remove(userAccount))
			{
				_iniData.Sections["Group"]["Members"] = String.Join(",", _members.Select(u => u.AccountName));
				Save();
			}
		}

		/// <inheritdoc/>
		public bool HasMember(IUserAccount userAccount)
		{
			return _members.Contains(userAccount);
		}

		/// <inheritdoc/>
		public void AddPermission(IPermission permission)
		{
			if (!_permissions.Contains(permission))
			{
				_permissions.Add(permission);
				_iniData.Sections["Group"]["Permissions"] = String.Join(",", _permissions);
				Save();
			}
		}

		/// <inheritdoc/>
		public void RemovePermission(IPermission permission)
		{
			if (_permissions.Remove(permission))
			{
				_iniData.Sections["Group"]["Permissions"] = String.Join(",", _permissions);
				Save();
			}
		}

		/// <inheritdoc/>
		public bool HasPermission(IPermission permission)
		{
			return _permissions.Contains(permission);
		}

		/// <summary>
		/// Saves this plain text group data to file in the pre-computed location.
		/// </summary>
		public void Save()
		{
			using (FileStream fs = new FileStream(GroupPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			{
				ToStream(fs);
			}
		}

		/// <summary>
		/// Saves this plain text group data into the specified <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> to save the data to.</param>
		public void ToStream(Stream stream)
		{
			using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true))
			{
				new FileIniDataParser().WriteData(sw, _iniData);
			}
		}
	}
}
