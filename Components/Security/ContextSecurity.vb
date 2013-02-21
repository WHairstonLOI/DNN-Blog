﻿'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Modules.Blog.Entities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Modules.Blog.Entities.Blogs
Imports DotNetNuke.Modules.Blog.Common.Constants

Namespace Security

 Public Class ContextSecurity
  Private Property IsOwner As Boolean = False
  Private _canAdd As Boolean = False
  Private _canEdit As Boolean = False
  Private _canApprove As Boolean = False
  Private _userIsAdmin As Boolean = False
  Private _isBlogger As Boolean = False
  Private _isEditor As Boolean = False

  Public Sub New(moduleId As Integer, tabId As Integer, blog As BlogInfo, user As UserInfo)
   If blog IsNot Nothing Then
    IsOwner = CBool(blog.CreatedByUserId = user.UserID)
    _canAdd = blog.CanAdd
    _canEdit = blog.CanEdit
    _canApprove = blog.CanApprove
   Else
    Using ir As IDataReader = Data.DataProvider.Instance().GetUserPermissionsByModule(moduleId, user.UserID)
     Do While ir.Read
      Dim permissionId As Integer = CInt(ir.Item("PermissionId"))
      Dim hasPermission As Integer = CInt(ir.Item("HasPermission"))
      If hasPermission > 0 Then
       Select Case permissionId
        Case BlogPermissionTypes.ADD
         _canAdd = True
        Case BlogPermissionTypes.EDIT
         _canEdit = True
        Case BlogPermissionTypes.APPROVE
         _canApprove = True
       End Select
      End If
     Loop
    End Using
   End If
   _userIsAdmin = DotNetNuke.Security.PortalSecurity.IsInRole(DotNetNuke.Entities.Portals.PortalSettings.Current.AdministratorRoleName)
   Dim mc As New DotNetNuke.Entities.Modules.ModuleController
   Dim objMod As New DotNetNuke.Entities.Modules.ModuleInfo
   objMod = mc.GetModule(moduleId, tabId, False)
   If objMod IsNot Nothing Then
    _isBlogger = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, Common.Constants.BloggerPermission)
    _isEditor = ModulePermissionController.HasModulePermission(objMod.ModulePermissions, "EDIT")
   End If
  End Sub

  Public ReadOnly Property CanEditEntry() As Boolean
   Get
    Return _canEdit Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanAddEntry() As Boolean
   Get
    Return _canAdd Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property CanApproveEntry() As Boolean
   Get
    Return _canApprove Or IsOwner Or UserIsAdmin
   End Get
  End Property

  Public ReadOnly Property UserIsAdmin As Boolean
   Get
    Return _userIsAdmin
   End Get
  End Property

  Public ReadOnly Property IsBlogger As Boolean
   Get
    Return _isBlogger
   End Get
  End Property

  Public ReadOnly Property IsEditor As Boolean
   Get
    Return _isEditor
   End Get
  End Property
 End Class
End Namespace