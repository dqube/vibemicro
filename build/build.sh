#!/bin/bash

# BuildingBlocks Build Script
# Usage: ./build.sh [target] [configuration] [options]

set -e

# Default values
CONFIGURATION="Release"
TARGET="Build"
SKIP_TESTS=false
CREATE_PACKAGES=false
PUBLISH_ARTIFACTS=false
OUTPUT_PATH=""
PACKAGE_VERSION=""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Get script directory and root directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
ARTIFACTS_DIR="$ROOT_DIR/artifacts"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        -t|--target)
            TARGET="$2"
            shift 2
            ;;
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        --create-packages)
            CREATE_PACKAGES=true
            shift
            ;;
        --publish-artifacts)
            PUBLISH_ARTIFACTS=true
            shift
            ;;
        --output-path)
            OUTPUT_PATH="$2"
            shift 2
            ;;
        --package-version)
            PACKAGE_VERSION="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [options]"
            echo "Options:"
            echo "  -c, --configuration    Build configuration (Debug|Release) [default: Release]"
            echo "  -t, --target          Build target (clean|restore|build|pack|publish|ci) [default: build]"
            echo "  --skip-tests          Skip running tests"
            echo "  --create-packages     Create NuGet packages"
            echo "  --publish-artifacts   Publish application artifacts"
            echo "  --output-path         Custom output path"
            echo "  --package-version     Package version override"
            echo "  -h, --help           Show this help message"
            exit 0
            ;;
        *)
            if [[ -z "$TARGET" ]]; then
                TARGET="$1"
            fi
            shift
            ;;
    esac
done

echo -e "${CYAN}🏗️ BuildingBlocks Build Script${NC}"
echo -e "${CYAN}==============================${NC}"
echo -e "${YELLOW}Configuration: $CONFIGURATION${NC}"
echo -e "${YELLOW}Target: $TARGET${NC}"
echo -e "${YELLOW}Root Directory: $ROOT_DIR${NC}"

# Clean artifacts directory
if [ -d "$ARTIFACTS_DIR" ]; then
    echo -e "${GREEN}🧹 Cleaning artifacts directory...${NC}"
    rm -rf "$ARTIFACTS_DIR"
fi

# Create artifacts directory
mkdir -p "$ARTIFACTS_DIR"

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check if .NET is installed
if ! command_exists dotnet; then
    echo -e "${RED}❌ .NET SDK is not installed or not in PATH${NC}"
    exit 1
fi

# Main build logic
case "${TARGET,,}" in
    "clean")
        echo -e "${GREEN}🧹 Cleaning solution...${NC}"
        dotnet clean "$ROOT_DIR" --configuration "$CONFIGURATION" --verbosity minimal
        ;;
    
    "restore")
        echo -e "${GREEN}📦 Restoring NuGet packages...${NC}"
        dotnet restore "$ROOT_DIR" --verbosity minimal
        ;;
    
    "build")
        echo -e "${GREEN}🔨 Building solution...${NC}"
        dotnet build "$ROOT_DIR" --configuration "$CONFIGURATION" --no-restore --verbosity minimal
        
        if [ "$SKIP_TESTS" = false ]; then
            echo -e "${GREEN}🧪 Running tests...${NC}"
            dotnet test "$ROOT_DIR" \
                --configuration "$CONFIGURATION" \
                --no-build \
                --verbosity minimal \
                --logger "trx" \
                --results-directory "$ARTIFACTS_DIR/test-results"
            
            # Generate code coverage report
            echo -e "${GREEN}📊 Generating code coverage report...${NC}"
            dotnet test "$ROOT_DIR" \
                --configuration "$CONFIGURATION" \
                --no-build \
                --collect:"XPlat Code Coverage" \
                --results-directory "$ARTIFACTS_DIR/coverage"
        fi
        ;;
    
    "pack")
        echo -e "${GREEN}📦 Creating NuGet packages...${NC}"
        PACK_ARGS="pack $ROOT_DIR --configuration $CONFIGURATION --no-build --output $ARTIFACTS_DIR/packages --verbosity minimal"
        
        if [ -n "$PACKAGE_VERSION" ]; then
            PACK_ARGS="$PACK_ARGS --property:PackageVersion=$PACKAGE_VERSION"
        fi
        
        dotnet $PACK_ARGS
        ;;
    
    "publish")
        echo -e "${GREEN}🚀 Publishing applications...${NC}"
        
        # Find all executable projects
        find "$ROOT_DIR" -name "*.csproj" -type f | while read -r project; do
            if grep -q -E "(<OutputType>Exe</OutputType>|Microsoft\.NET\.Sdk\.Web)" "$project"; then
                project_name=$(basename "$project" .csproj)
                publish_path="$ARTIFACTS_DIR/publish/$project_name"
                
                echo -e "${YELLOW}Publishing $project_name...${NC}"
                
                dotnet publish "$project" \
                    --configuration "$CONFIGURATION" \
                    --no-build \
                    --output "$publish_path" \
                    --verbosity minimal
            fi
        done
        ;;
    
    "ci")
        echo -e "${GREEN}🔄 Running CI build...${NC}"
        
        # Restore
        echo -e "${YELLOW}📦 Restoring packages...${NC}"
        dotnet restore "$ROOT_DIR" --verbosity minimal
        
        # Build
        echo -e "${YELLOW}🔨 Building...${NC}"
        dotnet build "$ROOT_DIR" --configuration "$CONFIGURATION" --no-restore --verbosity minimal
        
        # Test
        echo -e "${YELLOW}🧪 Running tests...${NC}"
        dotnet test "$ROOT_DIR" \
            --configuration "$CONFIGURATION" \
            --no-build \
            --verbosity minimal \
            --logger "trx" \
            --results-directory "$ARTIFACTS_DIR/test-results" \
            --collect:"XPlat Code Coverage" \
            --results-directory "$ARTIFACTS_DIR/coverage"
        
        # Pack
        if [ "$CREATE_PACKAGES" = true ]; then
            echo -e "${YELLOW}📦 Creating packages...${NC}"
            dotnet pack "$ROOT_DIR" \
                --configuration "$CONFIGURATION" \
                --no-build \
                --output "$ARTIFACTS_DIR/packages" \
                --verbosity minimal
        fi
        
        # Publish
        if [ "$PUBLISH_ARTIFACTS" = true ]; then
            echo -e "${YELLOW}🚀 Publishing artifacts...${NC}"
            "$SCRIPT_DIR/build.sh" --target publish --configuration "$CONFIGURATION"
        fi
        ;;
    
    *)
        echo -e "${RED}❌ Unknown target: $TARGET${NC}"
        echo "Valid targets are: clean, restore, build, pack, publish, ci"
        exit 1
        ;;
esac

echo -e "${GREEN}✅ Build completed successfully!${NC}"

# Display artifacts summary
if [ -d "$ARTIFACTS_DIR" ]; then
    echo -e "\n${CYAN}📁 Artifacts Summary:${NC}"
    find "$ARTIFACTS_DIR" -type f | \
        sed "s|$ARTIFACTS_DIR/||" | \
        cut -d'/' -f1 | \
        sort | \
        uniq -c | \
        while read -r count dir; do
            echo -e "  ${YELLOW}📂 $dir ($count files)${NC}"
        done
fi 