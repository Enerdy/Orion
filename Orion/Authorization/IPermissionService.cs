using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orion.Framework;

namespace Orion.Authorization
{
	/// <summary>
	/// Provides a mechanism for managing <see cref="IPermission"/> instances.
	/// Manages the permission tree.
	/// </summary>
	public interface IPermissionService : ISharedService
	{
		/// <summary>
		/// Returns the full permission tree, optionally filtered by a predicate expression.
		/// </summary>
		/// <param name="predicate">(optional) A predicate expression for filtering permissions.</param>
		/// <returns>
		/// An enumerable of <see cref="IPermission"/> objects satisfying the supplied predicate.  If no predicate
		/// is specified, an enumerable of all permissions is returned.
		/// </returns>
		IEnumerable<IPermission> FindPermissions(Predicate<IPermission> predicate = null);

		/// <summary>
		/// Gets a permission object from the tree based on its full node.
		/// </summary>
		/// <example>core.maintenance.groups</example>
		/// <param name="permissionNode">The full permission node to seek.</param>
		/// <returns>
		/// The corresponding <see cref="IPermission"/> object, or the compiler default if the tree does not contain
		/// this node.
		/// </returns>
		IPermission GetPermissionOrDefault(string permissionNode);

		/// <summary>
		/// Builds a permission node by recursively seeking its root.
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		string ToNode(IPermission permission);
	}
}
