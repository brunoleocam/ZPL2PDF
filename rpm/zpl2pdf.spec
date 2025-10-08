Name:           zpl2pdf
Version:        2.0.0
Release:        1%{?dist}
Summary:        ZPL to PDF Converter

License:        MIT
URL:            https://github.com/brunoleocam/ZPL2PDF
Source0:        https://github.com/brunoleocam/ZPL2PDF/archive/v%{version}.tar.gz

BuildArch:      noarch
BuildRequires:  dotnet-sdk-9.0
Requires:       libgdiplus
Requires:       glibc

%description
A powerful, cross-platform command-line tool that converts ZPL (Zebra Programming Language) files to high-quality PDF documents.

Features:
- Dual operation modes: Conversion and Daemon
- Intelligent dimension handling with automatic ZPL extraction
- Cross-platform support (Windows, Linux, macOS)
- Clean Architecture with SOLID principles
- High performance with async processing
- Enterprise-ready with PID management and logging

The tool supports both individual file conversion and automatic folder monitoring for batch processing. It can extract dimensions directly from ZPL commands (^PW, ^LL) and supports multiple units of measurement.

%prep
%setup -q

%build
dotnet publish ZPL2PDF.csproj --configuration Release --runtime linux-x64 --self-contained true --output %{_builddir}/%{name}-%{version}

%install
mkdir -p %{buildroot}%{_bindir}
mkdir -p %{buildroot}%{_datadir}/%{name}
mkdir -p %{buildroot}%{_mandir}/man1
mkdir -p %{buildroot}%{_docdir}/%{name}

cp %{_builddir}/%{name}-%{version}/ZPL2PDF %{buildroot}%{_bindir}/
cp -r %{_builddir}/%{name}-%{version}/* %{buildroot}%{_datadir}/%{name}/

# Create man page
cat > %{buildroot}%{_mandir}/man1/zpl2pdf.1 << 'EOF'
.TH ZPL2PDF 1 "2024" "ZPL2PDF 2.0.0" "User Commands"
.SH NAME
zpl2pdf - ZPL to PDF Converter
.SH SYNOPSIS
.B zpl2pdf
[\fIOPTIONS\fR]
.SH DESCRIPTION
ZPL2PDF is a powerful, cross-platform command-line tool that converts ZPL (Zebra Programming Language) files to high-quality PDF documents.
.SH OPTIONS
.TP
\fB\-i\fR, \fB\-\-input\fR
Input ZPL file (.txt or .prn)
.TP
\fB\-z\fR, \fB\-\-zpl\fR
ZPL content as string
.TP
\fB\-o\fR, \fB\-\-output\fR
Output folder for PDF
.TP
\fB\-n\fR, \fB\-\-name\fR
Output PDF filename (optional)
.TP
\fB\-w\fR, \fB\-\-width\fR
Label width
.TP
\fB\-h\fR, \fB\-\-height\fR
Label height
.TP
\fB\-u\fR, \fB\-\-unit\fR
Unit (mm, cm, in)
.TP
\fB\-d\fR, \fB\-\-dpi\fR
Print density (203, 300, etc.)
.TP
\fB\-l\fR, \fB\-\-listen\fR
Folder to monitor (daemon mode)
.TP
\fBstart\fR
Start daemon mode
.TP
\fBstop\fR
Stop daemon mode
.TP
\fBstatus\fR
Check daemon status
.TP
\fB\-h\fR, \fB\-\-help\fR
Show help message
.SH EXAMPLES
.TP
Convert a single file:
.B zpl2pdf -i label.txt -o output_folder -n my_label.pdf
.TP
Convert with custom dimensions:
.B zpl2pdf -i label.txt -o output_folder -w 10 -h 5 -u cm
.TP
Start daemon mode:
.B zpl2pdf start -l /path/to/watch -w 7.5 -h 15 -u in
.TP
Check daemon status:
.B zpl2pdf status
.SH AUTHOR
Bruno Leonardo Camargos
.SH SEE ALSO
https://github.com/brunoleocam/ZPL2PDF
EOF

%files
%{_bindir}/ZPL2PDF
%{_datadir}/%{name}/
%{_mandir}/man1/zpl2pdf.1
%{_docdir}/%{name}/

%changelog
* Mon Jan 01 2024 Bruno Leonardo Camargos <brunoleocam@example.com> - 2.0.0-1
- Initial release of ZPL2PDF 2.0.0
- Added daemon mode for automatic folder monitoring
- Implemented Clean Architecture with SOLID principles
- Added cross-platform support for Windows, Linux, and macOS
- Added intelligent dimension handling with ZPL extraction
- Added enterprise features like PID management and logging
