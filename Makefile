TEST_RESULT = unity-test-result.xml

debug:
	$(UNITY3D_PATH)/Editor/Unity -runTests -batchmode -testPlatform playmode -testResults $(TEST_RESULT) -logfile -

test:
	-$(UNITY3D_PATH)/Editor/Unity -runTests -batchmode -testPlatform playmode -testResults $(TEST_RESULT) -logfile - 2> /dev/null | grep VIRCADIA_SDK_TEST_LOG
	@xmlstarlet sel -t -m '//message[1]' -v . -n < $(TEST_RESULT)
	@xmlstarlet sel -t -m '//stack-trace[1]' -v . -n < $(TEST_RESULT)

clean-tests:
	-rm -rf $(TEST_RESULT)


.PHONY: test clean-tests
