@echo off
echo Running Book Lending App Tests...
echo.

cd test

echo Installing test dependencies...
dotnet restore

echo.
echo Running all tests with coverage (excluding migrations)...
dotnet test --collect:"XPlat Code Coverage" --settings:coverlet.runsettings --results-directory:Coverage\TestResults

echo.
echo Checking for coverage files...
dir Coverage\TestResults /s /b *.xml

echo.
echo Generating HTML coverage report...
dotnet tool install -g dotnet-reportgenerator-globaltool 2>nul
for /r Coverage\TestResults %%f in (coverage.cobertura.xml) do (
    if exist "%%f" (
        echo Found coverage file: %%f
        reportgenerator -reports:"%%f" -targetdir:"Coverage\Report" -reporttypes:Html
        echo.
        echo HTML Coverage Report Generated Successfully!
        echo Location: test\Coverage\Report\index.html
        goto :found
    )
)
echo No coverage files found!
:found

echo.
echo Opening coverage report in browser...
start Coverage\Report\index.html
echo.
echo Press any key to exit...
pause >nul