for /r %%d in (.) do if exist "%%d\bin" rd /s /q "%%d\bin"
for /r %%d in (.) do if exist "%%d\obj" rd /s /q "%%d\obj"
pause