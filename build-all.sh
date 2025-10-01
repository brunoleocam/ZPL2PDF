#!/bin/bash
# ZPL2PDF - Script de Build Cross-Platform para Linux/macOS
# Bash Script para compilar releases para todas as plataformas

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Função para escrever mensagens coloridas
print_color() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Função para mostrar ajuda
show_help() {
    print_color $CYAN "🚀 ZPL2PDF - Script de Build Cross-Platform"
    print_color $CYAN "============================================="
    echo ""
    print_color $CYAN "Uso: ./build-all.sh [opções]"
    echo ""
    print_color $CYAN "Opções:"
    print_color $CYAN "  -c, --config <config>    Configuração de build (Debug|Release) [Padrão: Release]"
    print_color $CYAN "  -o, --output <dir>       Diretório de saída [Padrão: bin/Release]"
    print_color $CYAN "  --clean                  Limpar builds anteriores"
    print_color $CYAN "  --test                   Executar testes após build"
    print_color $CYAN "  -h, --help               Mostrar esta ajuda"
    echo ""
    print_color $CYAN "Exemplos:"
    print_color $CYAN "  ./build-all.sh                           # Build Release para todas as plataformas"
    print_color $CYAN "  ./build-all.sh -c Debug                  # Build Debug"
    print_color $CYAN "  ./build-all.sh --clean --test            # Limpar e testar"
    echo ""
}

# Função para executar comando e verificar resultado
run_command() {
    local command=$1
    local description=$2
    
    print_color $CYAN "🔨 $description..."
    print_color $CYAN "   Comando: $command"
    
    eval $command
    local exit_code=$?
    
    if [ $exit_code -eq 0 ]; then
        print_color $GREEN "   ✅ $description concluído com sucesso!"
        return 0
    else
        print_color $RED "   ❌ $description falhou com código de saída: $exit_code"
        return 1
    fi
}

# Função para criar diretório se não existir
ensure_directory() {
    local path=$1
    if [ ! -d "$path" ]; then
        mkdir -p "$path"
        print_color $CYAN "   📁 Diretório criado: $path"
    fi
}

# Função para copiar arquivos de configuração
copy_config_files() {
    local target_dir=$1
    
    local config_files=("zpl2pdf.json" "README.md" "LICENSE")
    
    for file in "${config_files[@]}"; do
        if [ -f "$file" ]; then
            cp "$file" "$target_dir"
            print_color $CYAN "   📄 Copiado: $file"
        fi
    done
}

# Função para criar arquivo de informações da versão
create_version_info() {
    local target_dir=$1
    local platform=$2
    
    cat > "$target_dir/VERSION.txt" << EOF
# ZPL2PDF - Conversor ZPL para PDF
## Versão: $(date +%Y.%m.%d)
## Plataforma: $platform
## Build: $CONFIGURATION
## Data: $(date '+%Y-%m-%d %H:%M:%S')

## Comandos Disponíveis:

### Modo Conversão:
./ZPL2PDF -i arquivo.txt -o pasta -n nome.pdf -w 7.5 -h 15 -u in

### Modo Daemon:
./ZPL2PDF start                                    # Pasta padrão, extrai do ZPL
./ZPL2PDF start -l "/path/to/folder"              # Pasta customizada
./ZPL2PDF start -w 7.5 -h 15 -u in                # Parâmetros fixos
./ZPL2PDF stop                                     # Parar daemon
./ZPL2PDF status                                   # Verificar status

## Instalação:
1. Extrair arquivos para pasta desejada
2. Executar ./ZPL2PDF --help para ver opções
3. Para modo daemon, criar pasta de monitoramento

## Suporte:
- Windows: Pasta padrão em %USERPROFILE%\\Documents\\ZPL2PDF Auto Converter
- Linux: Pasta padrão em \$HOME/Documents/ZPL2PDF Auto Converter
EOF
    
    print_color $CYAN "   📝 Criado: VERSION.txt"
}

# Função para tornar executável
make_executable() {
    local file=$1
    if [ -f "$file" ]; then
        chmod +x "$file"
        print_color $CYAN "   🔧 Tornado executável: $(basename $file)"
    fi
}

# Valores padrão
CONFIGURATION="Release"
OUTPUT_DIR="bin/Release"
CLEAN=false
TEST=false

# Processar argumentos
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--config)
            CONFIGURATION="$2"
            shift 2
            ;;
        -o|--output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        --clean)
            CLEAN=true
            shift
            ;;
        --test)
            TEST=true
            shift
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            print_color $RED "❌ Opção desconhecida: $1"
            show_help
            exit 1
            ;;
    esac
done

# Início do script
print_color $CYAN "🚀 ZPL2PDF - Iniciando Build Cross-Platform"
print_color $CYAN "==========================================="
print_color $CYAN "Configuração: $CONFIGURATION"
print_color $CYAN "Diretório de saída: $OUTPUT_DIR"
echo ""

# Verificar se dotnet está instalado
print_color $CYAN "🔍 Verificando .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    print_color $RED "❌ .NET SDK não encontrado! Instale o .NET 9.0 SDK."
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
print_color $GREEN "✅ .NET SDK encontrado: $DOTNET_VERSION"
echo ""

# Limpar builds anteriores se solicitado
if [ "$CLEAN" = true ]; then
    print_color $CYAN "🧹 Limpando builds anteriores..."
    if [ -d "$OUTPUT_DIR" ]; then
        rm -rf "$OUTPUT_DIR"
        print_color $GREEN "   ✅ Diretório $OUTPUT_DIR removido"
    fi
    if [ -d "bin" ]; then
        rm -rf "bin"
        print_color $GREEN "   ✅ Diretório bin removido"
    fi
    echo ""
fi

# Executar testes se solicitado
if [ "$TEST" = true ]; then
    print_color $CYAN "🧪 Executando testes..."
    if ! run_command "dotnet test --configuration $CONFIGURATION --verbosity quiet" "Testes"; then
        print_color $RED "❌ Testes falharam! Build cancelado."
        exit 1
    fi
    echo ""
fi

# Definir plataformas para build
declare -a platforms=(
    "win-x64:Windows x64:ZPL2PDF.exe"
    "win-x86:Windows x86:ZPL2PDF.exe"
    "linux-x64:Linux x64:ZPL2PDF"
    "linux-arm64:Linux ARM64:ZPL2PDF"
    "linux-arm:Linux ARM:ZPL2PDF"
    "osx-x64:macOS x64:ZPL2PDF"
    "osx-arm64:macOS ARM64:ZPL2PDF"
)

success_count=0
total_count=${#platforms[@]}

# Build para cada plataforma
for platform_info in "${platforms[@]}"; do
    IFS=':' read -r rid name exe_name <<< "$platform_info"
    
    print_color $CYAN "🏗️  Buildando $name..."
    print_color $CYAN "   RID: $rid"
    
    platform_dir="$OUTPUT_DIR/$rid"
    ensure_directory "$platform_dir"
    
    build_command="dotnet publish -c $CONFIGURATION -r $rid --self-contained true -o \"$platform_dir\" -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:IncludeNativeLibrariesForSelfExtract=true"
    
    if run_command "$build_command" "Build $name"; then
        # Copiar arquivos de configuração
        copy_config_files "$platform_dir"
        
        # Criar arquivo de versão
        create_version_info "$platform_dir" "$name"
        
        # Verificar se executável foi criado
        exe_path="$platform_dir/$exe_name"
        if [ -f "$exe_path" ]; then
            file_size=$(du -h "$exe_path" | cut -f1)
            print_color $GREEN "   📦 Executável: $exe_name ($file_size)"
            
            # Tornar executável para plataformas Unix
            if [[ $rid == linux-* ]] || [[ $rid == osx-* ]]; then
                make_executable "$exe_path"
            fi
            
            ((success_count++))
        else
            print_color $RED "   ❌ Executável não encontrado: $exe_path"
        fi
    fi
    
    echo ""
done

# Resumo final
print_color $CYAN "📊 Resumo do Build"
print_color $CYAN "=================="
print_color $GREEN "✅ Sucessos: $success_count/$total_count"
print_color $RED "❌ Falhas: $((total_count - success_count))/$total_count"
echo ""

if [ $success_count -eq $total_count ]; then
    print_color $GREEN "🎉 Todos os builds foram concluídos com sucesso!"
    print_color $CYAN "📁 Releases disponíveis em: $OUTPUT_DIR"
    echo ""
    print_color $CYAN "📋 Estrutura de saída:"
    for dir in "$OUTPUT_DIR"/*/; do
        if [ -d "$dir" ]; then
            platform_name=$(basename "$dir")
            exe_name="ZPL2PDF"
            if [[ $platform_name == win-* ]]; then
                exe_name="ZPL2PDF.exe"
            fi
            exe_path="$dir$exe_name"
            if [ -f "$exe_path" ]; then
                size=$(du -h "$exe_path" | cut -f1)
                print_color $CYAN "   📦 $platform_name/$exe_name ($size)"
            fi
        fi
    done
else
    print_color $YELLOW "Alguns builds falharam. Verifique os erros acima."
    exit 1
fi

echo ""
print_color $GREEN "🚀 Build concluído!"
