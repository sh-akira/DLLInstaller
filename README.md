# DLLInstaller
C# COM DLL Installer like regsvr32  
regsvr32と同じ動作をC#で行うものです。  
COM DLLをWindowsに登録したい場合に、通常regsvr32で行いますが、  
ユーザーに難しいメッセージを出したくない場合や、分かりやすいメッセージを  
表示したい際に自由に書き換えて使うことが出来ます。  
登録には管理者権限が必要になりますが、通常起動しても自動で管理者権限を  
取得しなおして実行するようになっています。  
また、サイレントモードが実装されていて、メッセージボックスの非表示が出来ます。  
その場合アプリの終了コードが0かどうかで成功したか失敗したか判断できます。
成功時の終了コードは0です。マイナスが返ってくる場合失敗しています。

# Usage
```
Usage : DLLInstaller [/i or /u] [/s] [DLL file path].
    /i Install
    /u Uninstall
    /s Silent (Disable result message box)
Example :
DLLInstaller /i C:\comapi.dll
```

# Download
[Download from Release Page](https://github.com/sh-akira/DLLInstaller/releases)