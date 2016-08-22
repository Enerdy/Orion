using System;
using Orion.Framework;
using System.Collections.Generic;

namespace Orion.Authorization
{
	/// <summary>
	/// Provides a mechanism for managing <see cref="IGroup"/> instances.
	/// A group is a collection of settings shared between its members.
	/// </summary>
	public interface IGroupService : ISharedService
	{
		/// <summary>
		/// Gets the built-in administrator group.  That is, the group which members have access to everything.
		/// </summary>
		IGroup AdministratorGroup { get; }

		/// <summary>
		/// Gets the built-in anonymous group which all unauthenticated players are placed into.
		/// </summary>
		IGroup AnonymousGroup { get; }

		/// <summary>
		/// Finds a list of groups, optionally filtered by the supplied <paramref name="predicate"/>.
		/// </summary>
		/// <param name="predicate">
		/// (optional) A predicate expression to filter groups by.
		/// </param>
		/// <returns>
		/// An enumerable of all <see cref="IGroup"/> instances matching the specified <paramref name="predicate"/>.
		/// If no predicate expression was specified, returns all groups.
		/// </returns>
		IEnumerable<IGroup> FindGroups(Predicate<IGroup> predicate = null);

		/// <summary>
		/// Adds a group with the specified group name to the service, optionally with an initial list of members.
		/// </summary>
		/// <param name="groupName">
		/// A string containing the group name to add to the service.
		/// </param>
		/// <param name="initialMembers">
		/// (optional) A list of user accounts that will initially become members of this group.
		/// </param>
		IGroup AddGroup(string groupName, IEnumerable<IUserAccount> initialMembers = null);

		/// <summary>
		/// Deletes the specified <paramref name="group"/>.
		/// </summary>
		/// <param name="group">A reference to the group to be deleted.</param>
		void DeleteGroup(IGroup group);

		/// <summary>
		/// Binds the specified list of user accounts to the specified group.
		/// </summary>
		/// <param name="group">
		/// A reference to the group object to bind members to.
		/// </param>
		/// <param name="userAccounts">A list of user account objects to add to the specified group.</param>
		void AddMembers(IGroup group, params IUserAccount[] userAccounts);

		/// <summary>
		/// Ensures each permission in the specified list of permissions is added to the specified group.
		/// </summary>
		/// <param name="group">
		/// A reference to the group object to add permissions to.
		/// </param>
		/// <param name="permissions">A list of permission objects to add to the specified group.</param>
		void AddPermissions(IGroup group, params IPermission[] permissions);
	}
}
