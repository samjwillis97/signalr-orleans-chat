{ pkgs, ... }:
{
  packages = with pkgs; [ railway ];
  languages.dotnet.enable = true;
}
