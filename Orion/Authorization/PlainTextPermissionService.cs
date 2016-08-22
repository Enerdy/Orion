using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Orion.Framework;

namespace Orion.Authorization
{
	/// <summary>
	/// Plain text permission service which sources its account information from flat files in Orion's data
	/// subdirectory.
	/// </summary>
	[Service("Plain Text Permission Service", Author = "Nyx Studios")]
	public class PlainTextPermissionService : SharedService, IPermissionService
	{
		private List<PlainTextPermission> _permissions;

		/// <summary>
		/// The path to where <see cref="PlainTextPermission"/> objects are stored.
		/// </summary>
		public static string PermissionPathPrefix => Path.Combine("data", "permissions");

		/// <inheritdoc/>
		public PlainTextPermissionService(Orion orion) : base(orion)
		{
			Directory.CreateDirectory(PermissionPathPrefix);

			// Load permissions
			_permissions = new List<PlainTextPermission>();
			foreach (var filePath in Directory.GetFiles(PermissionPathPrefix, "*.ini"))
			{
				_permissions.Add(new PlainTextPermission(this));
			}
		}

		public IEnumerable<IPermission> FindPermissions(Predicate<IPermission> predicate = null)
		{
			throw new NotImplementedException();
		}

		public IPermission GetPermissionOrDefault(string permissionNode)
		{
			throw new NotImplementedException();
		}

		public string ToNode(IPermission permission)
		{
			throw new NotImplementedException();
		}
	}
}
