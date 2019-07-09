# List of TODO items for implementing Protobuf serialization in pact-net

- Perform proper healthcheck when starting serialization proxy, in stead of just sleeping.
- Refactor the PactBuilder class/interface to not pass a crazy function around.
- Use ProviderServiceInteractionFactory in the RegisterInteraction method of CustomSerializationMockProviderService.
- Provide a mechanism to enable the pact to be optionally created with the serialization proxy (so that not everyone needs to start the extra process if they don't require it).
- Add unit tests for Protobuf serialization.
- Add integration test which uses serialization proxy and Ruby core.
- Update examples to include a protobuf encoding.
- Package serialization proxy with ruby core as part of the nuget package.
