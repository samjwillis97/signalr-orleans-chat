{ pkgs, ... }:

{
  packages = with pkgs; [ railway ];
  languages.typescript.enable = true;
  languages.javascript.enable = true;
}
