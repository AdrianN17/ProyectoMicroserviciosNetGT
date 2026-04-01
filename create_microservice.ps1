<#
Crea una estructura de microservicio .NET con 4 proyectos y una solución.
Uso:
  .\create_microservice.ps1 -Name MiServicio
Opciones:
  -Force : eliminar la carpeta destino si existe
Notas:
  - Requiere `dotnet` en PATH.
  - Por defecto usa la plantilla `console` para todos los proyectos, si quieres que el API sea web use `dotnet new webapi` manualmente o modifica el script.
#>
param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$Name,
    [switch]$Force
)

function ExitOnLastError($message) {
    if ($LASTEXITCODE -ne 0) {
        Write-Error $message
        Pop-Location -ErrorAction SilentlyContinue
        exit $LASTEXITCODE
    }
}

# Verificar dotnet
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet no está instalado o no está en PATH. Instala .NET SDK y vuelve a intentar."
    exit 1
}

# Normalizar nombre
if ([string]::IsNullOrWhiteSpace($Name)) {
    Write-Error "El parámetro -Name no puede estar vacío."
    exit 1
}

$root = Join-Path -Path (Get-Location) -ChildPath $Name
if (Test-Path $root) {
    if ($Force) {
        Write-Host "La carpeta '$root' ya existe. Eliminando porque se pasó -Force..." -ForegroundColor Yellow
        Remove-Item -LiteralPath $root -Recurse -Force
    }
    else {
        Write-Error "La carpeta '$root' ya existe. Usa -Force para sobrescribir."
        exit 1
    }
}

Write-Host "Creando carpeta raíz: $root"
New-Item -ItemType Directory -Path $root | Out-Null
Push-Location $root

$projects = @("Api", "Application", "Domain", "Infraestructure")

foreach ($p in $projects) {
    $projName = "$Name.$p"
    Write-Host "Creando proyecto: $projName"
    # Por defecto usa console; si quieres webapi, cambia el template aquí para Api
    if ($p -eq 'Api') {
        # Cambia a webapi si prefieres una plantilla web
        dotnet new console -n $projName
    }
    else {
        dotnet new console -n $projName
    }
    ExitOnLastError "Fallo al crear el proyecto $projName"
}

Write-Host "Creando solución: $Name.sln"
# Crear solución con nombre del root
dotnet new sln -n $Name
ExitOnLastError "Fallo al crear la solución $Name.sln"

foreach ($p in $projects) {
    $projName = "$Name.$p"
    $csproj = Join-Path -Path $projName -ChildPath "$projName.csproj"
    Write-Host "Agregando proyecto a la solución: $csproj"
    dotnet sln add $csproj
    ExitOnLastError "Fallo al agregar $csproj a la solución"
}

Pop-Location
Write-Host "Hecho. Solución y proyectos creados en: $root" -ForegroundColor Green
Write-Host "Sugerencia: para un API real considera cambiar la plantilla del proyecto Api a 'webapi' (dotnet new webapi -n $Name.Api) y ajustar los templates en este script."

