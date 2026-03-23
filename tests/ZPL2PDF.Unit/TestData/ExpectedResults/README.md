# ExpectedResults (optional)

This folder is reserved for **golden-file** or reference outputs (e.g. PDF/PNG baselines) when you want binary regression checks beyond header/smoke validation.

- The default CI suite uses structural checks (PNG magic bytes, `%PDF-` header) and versioned ZPL under `TestData/ZplSuite/`.
- Add sample files here only when you intentionally lock renderer output to a specific hash or byte-for-byte baseline; document the generator version and update process in the PR that changes baselines.
