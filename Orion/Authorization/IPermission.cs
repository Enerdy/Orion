using System.Collections.Generic;
using Orion.Players;

namespace Orion.Authorization
{
	/// <summary>
	/// An authorization object which controls player access to Orion features, such as commands and
	/// custom plugin functionality.
	/// </summary>
	public interface IPermission
	{
		/// <summary>
		/// Gets the permission name. Must not contain any period (".") characters.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a human-readable description of the permission.
		/// </summary>
		/// <remarks>
		/// It is helpful to include what the permission is for in your description.
		/// </remarks>
		/// <example>
		/// "Enables access to the teleport command in tshock".
		/// </example>
		string Description { get; }

		/// <summary>
		/// Gets the parent permission in the permission tree.
		/// </summary>
		IPermission Parent { get; }

		/// <summary>
		/// Determines whether the specified user has this permission, or optionally any parent permission.
		/// </summary>
		/// <param name="userAccount">
		/// A reference to a user account object to check for permissions.
		/// </param>
		/// <param name="inherit">
		/// (optional) A flag indicating whether to check all parents of this permission for authorization.
		/// </param>
		/// <returns>
		/// true if the user has this permission, or optionally any parent permission, false otherwise.
		/// </returns>
		bool HasPermission(IUserAccount userAccount, bool inherit = true);
	}
}
