﻿using System;
using System.Collections.Generic;
using System.IO;
using Orion.Extensions;
using Orion.Framework;

namespace Orion.Authorization
{
	/// <summary>
	/// Plain text account and group service which sources its account information from flat files in Orion's data
	/// subdirectory.
	/// </summary>
	public class PlainTextAccountService : SharedService, IUserAccountService, IGroupService
	{
		private List<PlainTextGroup> _groups;

		/// <summary>
		/// The path to where <see cref="PlainTextUserAccount"/> objects are stored.
		/// </summary>
		public static string UserPathPrefix => Path.Combine("data", "users");

		/// <summary>
		/// The path to where <see cref="PlainTextGroup"/> objects are stored.
		/// </summary>
		public static string GroupPathPrefix => Path.Combine("data", "groups");

		/// <inheritdoc/>
		public PlainTextAccountService(Orion orion) : base(orion)
		{
			Directory.CreateDirectory(UserPathPrefix);
			Directory.CreateDirectory(GroupPathPrefix);

			// Load groups
			_groups = new List<PlainTextGroup>();
			foreach (var filePath in Directory.GetFiles(GroupPathPrefix, "*.ini"))
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					_groups.Add(new PlainTextGroup(this, fs));
				}
			}
		}

		/// <inheritdoc/>
		public IEnumerable<IUserAccount> FindAccounts(Predicate<IUserAccount> predicate = null)
		{
			foreach (var filePath in Directory.GetFiles(UserPathPrefix, "*.ini"))
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					PlainTextUserAccount userAccount = new PlainTextUserAccount(this, fs);

					if (predicate == null)
					{
						yield return userAccount;
					}
					else if (predicate(userAccount))
					{
						yield return userAccount;
					}
				}
			}
		}

		/// <inheritdoc/>
		public IUserAccount GetUserAccountOrDefault(string accountName)
		{
			string accountPath = Path.Combine(UserPathPrefix, $"{accountName.Slugify()}.ini");

			if (!File.Exists(accountPath))
			{
				return default(IUserAccount);
			}

			using (FileStream fs = new FileStream(accountPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return new PlainTextUserAccount(this, fs);
			}
		}

		/// <inheritdoc/>
		public IUserAccount AddAccount(string accountName)
		{
			PlainTextUserAccount userAccount;
			string accountPath;

			if (String.IsNullOrEmpty(accountName))
			{
				throw new ArgumentNullException(nameof(accountName));
			}

			accountPath = Path.Combine(UserPathPrefix, $"{accountName.Slugify()}.ini");

			if (File.Exists(accountPath))
			{
				throw new InvalidOperationException($"Account by the name of {accountName} already exists.");
			}

			userAccount = new PlainTextUserAccount(this)
			{
				AccountName = accountName
			};

			using (FileStream fs = new FileStream(accountPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			{
				userAccount.ToStream(fs);
			}

			return userAccount;
		}

		/// <inheritdoc/>
		public void DeleteAccount(string accountName)
		{
			string accountPath;

			if (String.IsNullOrEmpty(accountName))
			{
				throw new ArgumentNullException(nameof(accountName));
			}

			accountPath = Path.Combine(UserPathPrefix, $"{accountName.Slugify()}.ini");

			File.Delete(accountPath);
		}

		/// <inheritdoc/>
		public void SetPassword(IUserAccount userAccount, string password)
		{
			userAccount.SetPassword(password);
		}

		/// <inheritdoc/>
		public void ChangePassword(IUserAccount userAccount, string currentPassword, string newPassword)
		{
			userAccount.ChangePassword(currentPassword, newPassword);
		}

		/// <inheritdoc/>
		public bool Authenticate(IUserAccount userAccount, string password)
		{
			return userAccount.Authenticate(password);
		}

		/// <inheritdoc/>
		public IGroup AdministratorGroup { get; }

		/// <inheritdoc/>
		public IGroup AnonymousGroup { get; }

		/// <inheritdoc/>
		public IEnumerable<IGroup> FindGroups(Predicate<IGroup> predicate = null)
		{
			if (predicate == null)
			{
				return _groups;
			}

			return _groups.FindAll(predicate);
		}

		/// <inheritdoc/>
		public IGroup AddGroup(string groupName, IEnumerable<IUserAccount> initialMembers = null)
		{
			PlainTextGroup group;
			string groupPath;

			if (String.IsNullOrEmpty(groupName))
			{
				throw new ArgumentNullException(nameof(groupName));
			}

			groupPath = Path.Combine(GroupPathPrefix, $"{groupName.Slugify()}.ini");

			if (_groups.Exists(g => g.Name == groupName) || File.Exists(groupPath))
			{
				throw new InvalidOperationException($"Group by the name of {groupName} already exists.");
			}

			group = new PlainTextGroup(this)
			{
				Name = groupName
			};

			if (initialMembers != null)
			{
				foreach (IUserAccount a in initialMembers)
				{
					group.AddMember(a);
				}
			}

			_groups.Add(group);
			using (FileStream fs = new FileStream(groupPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
			{
				group.ToStream(fs);
			}

			return group;
		}

		/// <inheritdoc/>
		public void DeleteGroup(IGroup group)
		{
			string groupPath = Path.Combine(GroupPathPrefix, $"{group.Name.Slugify()}.ini");

			_groups.Remove((PlainTextGroup)group);
			File.Delete(groupPath);
		}

		/// <inheritdoc/>
		public void AddMembers(IGroup group, params IUserAccount[] userAccounts)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public void AddPermissions(IGroup group, params IPermission[] permissions)
		{
			throw new NotImplementedException();
		}
	}
}
